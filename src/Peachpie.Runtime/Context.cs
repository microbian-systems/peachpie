﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pchp.Core.Utilities;
using System.Reflection;
using Pchp.Core.Reflection;

namespace Pchp.Core
{
    /// <summary>
    /// Runtime context for a PHP application.
    /// </summary>
    /// <remarks>
    /// The object represents a current Web request or the application run.
    /// Its instance is passed to all PHP function.
    /// The context is not thread safe.
    /// </remarks>
    [DebuggerNonUserCode]
    public partial class Context : IDisposable
    {
        #region Create

        protected Context()
        {
            _functions = new RoutinesTable(RoutinesAppContext.NameToIndex, RoutinesAppContext.AppRoutines, RoutinesAppContext.ContextRoutinesCounter, FunctionRedeclared);
            _types = new TypesTable(TypesAppContext.NameToIndex, TypesAppContext.AppTypes, TypesAppContext.ContextTypesCounter, TypeRedeclared);
            _statics = new object[StaticIndexes.StaticsCount];

            // TODO: InitGlobalVariables(); //_globals.SetItemAlias(new IntStringKey("GLOBALS"), new PhpAlias(PhpValue.Create(_globals)));
            // TODO: wrap into a struct Superglobals

            _globals = new PhpArray();
            _server = new PhpArray();   // TODO: virtual initialization method, reuse server static information with request context
            _env = new PhpArray();
            _request = new PhpArray();
            _get = new PhpArray();
            _post = new PhpArray();
            _files = new PhpArray();
            _session = new PhpArray();
            _cookie = new PhpArray();
        }

        /// <summary>
        /// Create default context with no output.
        /// </summary>
        public static Context CreateEmpty()
        {
            var ctx = new Context();
            ctx.InitOutput(null);

            return ctx;
        }

        #endregion

        #region Symbols

        /// <summary>
        /// Map of global functions.
        /// </summary>
        readonly RoutinesTable _functions;

        /// <summary>
        /// Map of global types.
        /// </summary>
        readonly TypesTable _types;

        /// <summary>
        /// Map of global constants.
        /// </summary>
        readonly ConstsMap _constants = new ConstsMap();

        readonly ScriptsMap _scripts = new ScriptsMap();

        /// <summary>
        /// Internal method to be used by loader to load referenced symbols.
        /// </summary>
        /// <typeparam name="TScript"><c>&lt;Script&gt;</c> type in compiled assembly. The type contains static methods for enumerating referenced symbols.</typeparam>
        public static void AddScriptReference<TScript>() => AddScriptReference(typeof(TScript));

        /// <summary>
        /// Load PHP scripts and referenced symbols from PHP assembly.
        /// </summary>
        /// <param name="assembly">PHP assembly containing special <see cref="ScriptInfo.ScriptTypeName"/> class.</param>
        public static void AddScriptReference(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            var t = assembly.GetType(ScriptInfo.ScriptTypeName);
            if (t != null)
            {
                AddScriptReference(t);
            }
        }

        /// <summary>
        /// Reflects given <c>&lt;Script&gt;</c> type generated by compiler to load list of its symbols
        /// and make them available to runtime.
        /// </summary>
        /// <param name="tscript"><c>&lt;Script&gt;</c> type from compiled assembly.</param>
        protected static void AddScriptReference(Type tscript)
        {
            Debug.Assert(tscript != null);
            Debug.Assert(tscript.Name == ScriptInfo.ScriptTypeName);

            var tscriptinfo = tscript.GetTypeInfo();

            tscriptinfo.GetDeclaredMethod("EnumerateReferencedFunctions")
                .Invoke(null, new object[] { new Action<string, RuntimeMethodHandle>(RoutinesAppContext.DeclareRoutine) });

            tscriptinfo.GetDeclaredMethod("EnumerateScripts")
                .Invoke(null, new object[] { new Action<string, RuntimeMethodHandle>(ScriptsMap.DeclareScript) });

            tscriptinfo.GetDeclaredMethod("EnumerateConstants")
                .Invoke(null, new object[] { new Action<string, PhpValue, bool>(ConstsMap.DefineAppConstant) });
        }

        /// <summary>
        /// Declare a runtime user function.
        /// </summary>
        public void DeclareFunction(RoutineInfo routine) => _functions.DeclarePhpRoutine(routine);

        public void AssertFunctionDeclared(RoutineInfo routine)
        {
            if (!_functions.IsDeclared(routine))
            {
                // TODO: ErrCode function is not declared
            }
        }

        internal bool CheckFunctionDeclared(int index, RuntimeMethodHandle expected) => _functions.IsDeclared(index, expected);

        /// <summary>
        /// Gets declared function with given name. In case of more items they are considered as overloads.
        /// </summary>
        public RoutineInfo GetDeclaredFunction(string name) => _functions.GetDeclaredRoutine(name);

        /// <summary>
        /// Declare a runtime user type.
        /// </summary>
        /// <typeparam name="T">Type to be declared in current context.</typeparam>
        public void DeclareType<T>() => _types.DeclareType<T>();

        public void AssertTypeDeclared<T>()
        {
            if (!_types.IsDeclared(TypeInfoHolder<T>.TypeInfo))
            {
                // TODO: autoload, ErrCode
            }
        }

        /// <summary>
        /// Gets runtime type information, or <c>null</c> if type with given is not declared.
        /// </summary>
        public PhpTypeInfo GetDeclaredType(string name) => _types.GetDeclaredType(name);

        /// <summary>
        /// Gets enumeration of all types declared in current context.
        /// </summary>
        public IEnumerable<PhpTypeInfo> GetDeclaredTypes() => _types.GetDeclaredTypes();

        void FunctionRedeclared(RoutineInfo routine)
        {
            // TODO: ErrCode & throw
            throw new InvalidOperationException($"Function {routine.Name} redeclared!");
        }

        void TypeRedeclared(PhpTypeInfo type)
        {
            Debug.Assert(type != null);

            // TODO: ErrCode & throw
            throw new InvalidOperationException($"Type {type.Name} redeclared!");
        }

        #endregion

        #region Inclusions

        /// <summary>
        /// Used by runtime.
        /// Determines whether the <c>include_once</c> or <c>require_once</c> is allowed to proceed.
        /// </summary>
        public bool CheckIncludeOnce<TScript>() => !_scripts.IsIncluded<TScript>();

        /// <summary>
        /// Used by runtime.
        /// Called by scripts Main method at its begining.
        /// </summary>
        /// <typeparam name="TScript">Script type containing the Main method/</typeparam>
        public void OnInclude<TScript>() => _scripts.SetIncluded<TScript>();

        /// <summary>
        /// Resolves path according to PHP semantics, lookups the file in runtime tables and calls its Main method within the global scope.
        /// </summary>
        /// <param name="dir">Current script directory. Used for relative path resolution. Can be <c>null</c> to not resolve against current directory.</param>
        /// <param name="path">The relative or absolute path to resolve and include.</param>
        /// <param name="once">Whether to include according to include once semantics.</param>
        /// <param name="throwOnError">Whether to include according to require semantics.</param>
        /// <returns>Inclusion result value.</returns>
        public PhpValue Include(string dir, string path, bool once = false, bool throwOnError = false)
            => Include(dir, path, _globals, null, once, throwOnError);

        /// <summary>
        /// Resolves path according to PHP semantics, lookups the file in runtime tables and calls its Main method.
        /// </summary>
        /// <param name="cd">Current script directory. Used for relative path resolution. Can be <c>null</c> to not resolve against current directory.</param>
        /// <param name="path">The relative or absolute path to resolve and include.</param>
        /// <param name="locals">Variables scope for the included script.</param>
        /// <param name="this">Reference to <c>this</c> variable.</param>
        /// <param name="once">Whether to include according to include once semantics.</param>
        /// <param name="throwOnError">Whether to include according to require semantics.</param>
        /// <returns>Inclusion result value.</returns>
        public PhpValue Include(string cd, string path, PhpArray locals, object @this = null, bool once = false, bool throwOnError = false)
        {
            ScriptInfo script;

            path = path.Replace('/', '\\'); // normalize slashes
            if (path.StartsWith(this.RootPath, StringComparison.Ordinal)) // rooted
            {
                script = _scripts.GetScript(path.Substring(this.RootPath.Length));
            }
            else
            {
                script = ScriptsMap.SearchForIncludedFile(path, IncludePaths, cd, _scripts.GetScript);
            }
            if (script.IsValid)
            {
                if (once && _scripts.IsIncluded(script.Index))
                {
                    return PhpValue.Create(true);
                }
                else
                {
                    return script.MainMethod(this, locals, @this);
                }
            }
            else
            {
                if (throwOnError)
                {
                    throw new ArgumentException($"File '{path}' cannot be included with current configuration.");   // TODO: ErrCode
                }
                else
                {
                    return PhpValue.Create(false);   // TODO: Warning
                }
            }
        }

        #endregion

        #region Path Resolving

        /// <summary>
        /// Root directory (web root or console app root) where loaded scripts are relative to.
        /// </summary>
        /// <remarks>
        /// - <c>__FILE__</c> and <c>__DIR__</c> magic constants are resolved as concatenation with this value.
        /// </remarks>
        public virtual string RootPath { get; } = string.Empty;

        /// <summary>
        /// Current working directory.
        /// </summary>
        public virtual string WorkingDirectory { get; } = string.Empty;

        /// <summary>
        /// Set of include paths to be used to resolve full file path.
        /// </summary>
        public virtual string[] IncludePaths => null;   // TODO:  => this.Config.FileSystem.IncludePaths

        /// <summary>
        /// Gets full script path in current context.
        /// </summary>
        /// <typeparam name="TScript">Script type.</typeparam>
        /// <returns>Full script path.</returns>
        public string ScriptPath<TScript>() => RootPath + ScriptsMap.GetScript<TScript>().Path;

        #endregion

        #region Superglobals

        /// <summary>
        /// Array of global variables.
        /// Cannot be <c>null</c>.
        /// </summary>
        public PhpArray Globals
        {
            get { return _globals; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();  // TODO: ErrCode
                }

                _globals = value;
            }
        }
        PhpArray _globals;

        /// <summary>
        /// Array of server and execution environment information.
        /// Cannot be <c>null</c>.
        /// </summary>
        public PhpArray Server
        {
            get { return _server; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _server = value;
            }
        }
        PhpArray _server;

        /// <summary>
        /// An associative array of variables passed to the current script via the environment method.
        /// Cannot be <c>null</c>.
        /// </summary>
        public PhpArray Env
        {
            get { return _env; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _env = value;
            }
        }
        PhpArray _env;

        /// <summary>
        /// An array that by default contains the contents of $_GET, $_POST and $_COOKIE.
        /// Cannot be <c>null</c>.
        /// </summary>
        public PhpArray Request
        {
            get { return _request; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _request = value;
            }
        }
        PhpArray _request;

        /// <summary>
        /// An associative array of variables passed to the current script via the URL parameters.
        /// Cannot be <c>null</c>.
        /// </summary>
        public PhpArray Get
        {
            get { return _get; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _get = value;
            }
        }
        PhpArray _get;

        /// <summary>
        /// An associative array of variables passed to the current script via the HTTP POST method.
        /// Cannot be <c>null</c>.
        /// </summary>
        public PhpArray Post
        {
            get { return _post; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _post = value;
            }
        }
        PhpArray _post;

        /// <summary>
        /// An associative array of items uploaded to the current script via the HTTP POST method.
        /// Cannot be <c>null</c>.
        /// </summary>
        public PhpArray Files
        {
            get { return _files; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _files = value;
            }
        }
        PhpArray _files;

        /// <summary>
        /// An associative array containing session variables available to the current script.
        /// Cannot be <c>null</c>.
        /// </summary>
        public PhpArray Session
        {
            get { return _session; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _session = value;
            }
        }
        PhpArray _session;

        /// <summary>
        /// An associative array of variables passed to the current script via the HTTP POST method.
        /// Cannot be <c>null</c>.
        /// </summary>
        public PhpArray Cookie
        {
            get { return _cookie; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _cookie = value;
            }
        }
        PhpArray _cookie;

        #endregion

        #region Constants

        /// <summary>
        /// Gets a constant value.
        /// </summary>
        public PhpValue GetConstant(string name)
        {
            int idx = 0;
            return GetConstant(name, ref idx);
        }

        /// <summary>
        /// Gets a constant value.
        /// </summary>
        public PhpValue GetConstant(string name, ref int idx)
        {
            return _constants.GetConstant(name, ref idx);

            // TODO: check the constant is valid (PhpValue.IsSet) otherwise Warning: undefined constant
        }

        /// <summary>
        /// Defines a runtime constant.
        /// </summary>
        public bool DefineConstant(string name, PhpValue value, bool ignorecase = false) => _constants.DefineConstant(name, value, ignorecase);

        /// <summary>
        /// Determines whether a constant with given name is defined.
        /// </summary>
        public bool IsConstantDefined(string name) => _constants.IsDefined(name);

        /// <summary>
        /// Gets enumeration of all available constants and their values.
        /// </summary>
        public IEnumerable<KeyValuePair<string, PhpValue>> GetConstants() => _constants;

        #endregion

        #region Error Reporting

        /// <summary>
        /// Whether to throw an exception on soft error (Notice, Warning, Strict).
        /// </summary>
        public bool ThrowExceptionOnError { get; set; } = true;

        /// <summary>
        /// Gets whether error reporting is disabled or enabled.
        /// </summary>
        public bool ErrorReportingDisabled => _errorReportingDisabled != 0; // && !config.ErrorControl.IgnoreAtOperator;
        int _errorReportingDisabled = 0;

        /// <summary>
        /// Disables error reporting. Can be called for multiple times. To enable reporting again 
        /// <see cref="EnableErrorReporting"/> should be called as many times as <see cref="DisableErrorReporting"/> was.
        /// </summary>
        public void DisableErrorReporting()
        {
            _errorReportingDisabled++;
        }

        /// <summary>
        /// Enables error reporting disabled by a single call to <see cref="DisableErrorReporting"/>.
        /// </summary>
        public void EnableErrorReporting()
        {
            if (_errorReportingDisabled > 0)
                _errorReportingDisabled--;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            //if (!disposed)
            {
                try
                {
                    //this.GuardedCall<object, object>(this.ProcessShutdownCallbacks, null, false);
                    //this.GuardedCall<object, object>(this.FinalizePhpObjects, null, false);
                    FinalizeBufferedOutput();

                    //// additional disposal action
                    //if (this.TryDispose != null)
                    //    this.TryDispose();
                }
                finally
                {
                    //// additional disposal action
                    //if (this.FinallyDispose != null)
                    //    this.FinallyDispose();

                    ////
                    //this.disposed = true;
                }
            }
        }

        #endregion
    }
}
