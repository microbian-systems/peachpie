﻿using Pchp.Core;
using Pchp.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Pchp.Library.Reflection
{
    [PhpType("[name]"), PhpExtension(ReflectionUtils.ExtensionName)]
    public class ReflectionFunction : ReflectionFunctionAbstract
    {
        #region Construction

        protected ReflectionFunction() { }

        internal ReflectionFunction(RoutineInfo routine)
        {
            Debug.Assert(routine != null);
            _routine = routine;
        }

        public ReflectionFunction(Context ctx, PhpValue name)
        {
            __construct(ctx, name);
        }

        public void __construct(Context ctx, PhpValue name)
        {
            Debug.Assert(_routine == null, "Subsequent call not allowed.");

            object instance;
            var str = name.ToStringOrNull();
            if (str != null)
            {
                _routine = ctx.GetDeclaredFunction(str);
            }
            else if ((instance = name.AsObject()) != null)
            {
                if (instance is Closure)
                {
                    // _routine = ((Closure)instance).routine; // TODO: handle its $this parameter and use parameters
                    throw new NotImplementedException();
                }
            }

            if (_routine == null)
            {
                throw new ArgumentException();  // TODO: ReflectionException
            }
        }

        #endregion

        public static string export(string name, bool @return = false) { throw new NotImplementedException(); }
        public Closure getClosure() => Operators.BuildClosure(_routine, PhpArray.Empty, PhpArray.Empty);
        public PhpValue invoke(Context ctx, params PhpValue[] args) => _routine.PhpCallable(ctx, args);
        public PhpValue invokeArgs(Context ctx, PhpArray args) => _routine.PhpCallable(ctx, args.GetValues());
        public bool isDisabled() => false;
    }
}
