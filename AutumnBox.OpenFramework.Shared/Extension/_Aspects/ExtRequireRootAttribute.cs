﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/13 4:10:56 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.Basic.Device;
using AutumnBox.OpenFramework.Open;
using System;

namespace AutumnBox.OpenFramework.Extension
{
    /// <summary>
    /// 明确标记标明该拓展需要设备ROOT权限
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExtRequireRootAttribute : BeforeCreatingAspect, IInformationAttribute
    {
        private readonly bool reqRoot;
        /// <summary>
        /// key
        /// </summary>
        public string Key => ExtensionInformationKeys.ROOT;
        /// <summary>
        /// value
        /// </summary>
        public object Value => reqRoot;

        /// <summary>
        /// 构造
        /// </summary>
        public ExtRequireRootAttribute(bool reqRoot)
        {
            this.reqRoot = reqRoot;
        }
        /// <summary>
        /// 构造
        /// </summary>
        public ExtRequireRootAttribute() : this(true) { }

        private static bool DeviceHaveRoot(IDevice device)
        {
            return device.HaveSU();
        }
        /// <summary>
        /// 从前有座山
        /// </summary>
        /// <param name="args"></param>
        /// <param name="canContinue"></param>
        public override void BeforeCreating(BeforeCreatingAspectArgs args, ref bool canContinue)
        {
            IDevice selectedDevice = args.Context.GetService<IDeviceSelector>(ServicesNames.DEVICE_SELECTOR).GetCurrent(args.Context);
            if (!reqRoot || selectedDevice == null)
            {
                return;
            }
            if (DeviceHaveRoot(selectedDevice))
            {
                canContinue = true;
            }
            else
            {

                switch (args.Context.Ux.DoChoice("OpenFxNoRoot", "OpenFxNoRootIgnore"))
                {
                    case ChoiceResult.Left:
                        canContinue = true;
                        break;
                    default:
                        canContinue = false;
                        break;
                }
            }
        }
    }
}
