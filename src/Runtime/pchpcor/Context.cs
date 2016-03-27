﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class Context : IDisposable
    {
        #region Create

        private Context()
        {
        }

        /// <summary>
        /// Create context to be used within a console application.
        /// </summary>
        public static Context CreateConsole()
        {
            return new Context();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {

        }

        #endregion

        #region Echo

        public void Echo(object value)
        {
            if (value != null)
                Echo(value.ToString());
        }

        public void Echo(string value)
        {
            if (value != null)
                ConsoleImports.Write(value);
        }

        public void Echo(PhpString value)
        {
            Echo(value.ToString(this));    // TODO: echo string builder chunks to avoid concatenation
        }

        public void Echo(PhpValue value)
        {
            ConsoleImports.Write(value.ToString(this));
        }

        public void Echo(PhpNumber value)
        {
            if (value.IsLong)
                Echo(value.Long);
            else
                Echo(value.Double);
        }

        public void Echo(double value)
        {
            ConsoleImports.Write(Convert.ToString(value, this));
        }

        public void Echo(long value)
        {
            ConsoleImports.Write(value.ToString());
        }

        public void Echo(int value)
        {
            ConsoleImports.Write(value.ToString());
        }

        #endregion
    }
}
