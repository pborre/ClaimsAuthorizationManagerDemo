using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Riziv.BizTalk.Common
{
    public class ServiceRolesAndCertAuthZBehavior : IServiceBehavior
    {

        ADGroupAccessConfigurationCollection adGroupAccessList;
        CertAccessConfigurationCollection certAccessList;

        public ServiceRolesAndCertAuthZBehavior(ADGroupAccessConfigurationCollection _adGroupAccessList, CertAccessConfigurationCollection _certAccessList )
        {
            System.Diagnostics.EventLog.WriteEntry("WCF Authoriation manager", "behavior constr");
            this.adGroupAccessList = _adGroupAccessList;
            this.certAccessList = _certAccessList;
        }
        #region IServiceBehavior members
        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints,
            System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
           
        }
        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            
            System.Diagnostics.EventLog.WriteEntry("WCF Authoriation manager", "apply dispatch hooked");

            foreach (ChannelDispatcher disp in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher epDisp in disp.Endpoints)
                {
                    epDisp.DispatchRuntime.MessageInspectors.Add(new CallerIdentityInspector());
                }
            }

            ServiceAuthorizationBehavior authBehavior = serviceDescription.Behaviors.Find<ServiceAuthorizationBehavior>();
            authBehavior.ServiceAuthorizationManager = new ServiceRolesAndCertAuthZMgr(this.adGroupAccessList, this.certAccessList);
            ((IServiceBehavior)authBehavior).ApplyDispatchBehavior(serviceDescription, serviceHostBase); //The magic line. WCF does not need it, but guess who does..

        }
        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
        #endregion
        
    }
}
