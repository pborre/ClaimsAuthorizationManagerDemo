using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.BizTalk.Component.Interop;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;

namespace Riziv.BizTalk.Common
{
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Guid("af449b02-4108-4a1d-a452-2daaebcde1fd")]
    public class OriginalCallerForwarder : IBaseComponent, IComponent, IPersistPropertyBag, IComponentUI
    {
        #region IComponent Members

        public Microsoft.BizTalk.Message.Interop.IBaseMessage Execute(IPipelineContext pContext, Microsoft.BizTalk.Message.Interop.IBaseMessage pInMsg)
        {
            System.Diagnostics.EventLog.WriteEntry("BizTalk pipeline", "Writing original caller to the SOAP header");
            // original caller
            string originalCaller = "<headers>" + pInMsg.Context.Read("OriginalCaller", "http://www.riziv-inami.fgov.be/BizTalk/common").ToString() + "</headers>";

            System.Diagnostics.EventLog.WriteEntry("BizTalk pipeline", originalCaller);

            pInMsg.Context.Write("OutboundCustomHeaders", "http://schemas.microsoft.com/BizTalk/2006/01/Adapters/WCF-properties", originalCaller);

            return pInMsg;
        }

        #endregion

        #region IBaseComponent Members

        public string Description
        {
            get { return "Forward the original caller of a WCF endpoint"; }
        }

        public string Name
        {
            get { return "Original caller forwarder"; }
        }

        public string Version
        {
            get { return "1.0.0"; }
        }

        #endregion

        #region IPersistPropertyBag Members

        public void GetClassID(out Guid classID)
        {
            classID = new Guid("af449b02-4108-4a1d-a452-2daaebcde1fd");
        }

        public void InitNew()
        {
        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
        }

        #endregion

        #region IComponentUI Members

        public IntPtr Icon
        {
            get { return IntPtr.Zero; }
        }

        public System.Collections.IEnumerator Validate(object projectSystem)
        {
            return null;
        }

        #endregion
    }
}
