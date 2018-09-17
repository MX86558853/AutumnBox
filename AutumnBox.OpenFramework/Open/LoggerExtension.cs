﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/9/18 0:45:54 (UTC +8:00)
** desc： ...
*************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutumnBox.OpenFramework.Open
{
    public static class LoggerExtension
    {
        /// <summary>
        /// DEBUG LOG
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="content"></param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void CDebug(this ILogger logger,object content)
        {
            logger.Debug(content?.ToString());
        }
    }
}
