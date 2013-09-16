using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Security.Principal;

namespace Riziv.BizTalk.Common
{
    public class CallerIdentityInspector : IDispatchMessageInspector
    {

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            System.Diagnostics.EventLog.WriteEntry("IdentityResolver", "Resolving identity");

            WindowsIdentity user = null;
            string thumbPrint = string.Empty;
            
            if (ServiceSecurityContext.Current.WindowsIdentity != null)
            {
                if (ServiceSecurityContext.Current.WindowsIdentity.IsAuthenticated == true)
                {
                    user = ServiceSecurityContext.Current.WindowsIdentity;
                }
            }
            thumbPrint = Identitytools.GetClientCertificateThumbprint(ServiceSecurityContext.Current);

            if(null != user)
                request.Headers.Add(MessageHeader.CreateHeader("OriginalCaller", "http://www.riziv-inami.fgov.be/BizTalk/common", user.Name));
            if (string.Empty != thumbPrint)
                request.Headers.Add(MessageHeader.CreateHeader("OriginalCaller", "http://www.riziv-inami.fgov.be/BizTalk/common", thumbPrint));

            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        #endregion
    }

    
}
