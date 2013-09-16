using System;
using System.IdentityModel.Claims;
using System.Security.Principal;
using System.ServiceModel;

namespace Riziv.BizTalk.Common
{
    internal class ServiceRolesAndCertAuthZMgr : ServiceAuthorizationManager
    {
        ADGroupAccessConfigurationCollection adGroupAccessList;
        CertAccessConfigurationCollection certAccessList;

        public ServiceRolesAndCertAuthZMgr(ADGroupAccessConfigurationCollection _adGroupAccessList, CertAccessConfigurationCollection _certAccessList)
        {
            System.Diagnostics.EventLog.WriteEntry("WCF Authoriation manager", "construct");
            this.adGroupAccessList = _adGroupAccessList;
            this.certAccessList = _certAccessList;
        }
        
        /// <summary>
        /// Check access based upon AD Groups or certificates
        /// </summary>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public override bool CheckAccess(OperationContext operationContext)
        {
            System.Diagnostics.EventLog.WriteEntry("WCF Authoriation manager", "checkaccess called");
            WindowsIdentity user = null;
            WindowsPrincipal principal = null;
            string thumbPrint = string.Empty;

            if (operationContext.ServiceSecurityContext.WindowsIdentity != null)
            {
                if (operationContext.ServiceSecurityContext.WindowsIdentity.IsAuthenticated == true)
                {
                    user = operationContext.ServiceSecurityContext.WindowsIdentity;
                    principal = new WindowsPrincipal(user);

                    System.Diagnostics.Debug.WriteLine("ServiceRolesAndCertAuthZMgr - User: " + user);
                }
            }
            thumbPrint = Identitytools.GetClientCertificateThumbprint(operationContext.ServiceSecurityContext);
            System.Diagnostics.Debug.WriteLine("ServiceRolesAndCertAuthZMgr - Thumbpring: " + thumbPrint);

            string originalAction = operationContext.IncomingMessageHeaders.Action;
            string operation = originalAction.Substring(originalAction.LastIndexOf("/") + 1);
            string targetServiceNamespace = originalAction.Substring(0, originalAction.LastIndexOf("/"));
            string targetServiceNamespaceWithLastSlash = targetServiceNamespace + "/";

            //The following could be done with Linq (less lines of code) but then .NET 3.5 is prerequisite.
            //Not all servers have .NET 3.5 installed => no Linq
            //Check access based on Windows security (Groups)
            if (principal != null)
            {
                foreach (ADGroupAuthZElement action in this.adGroupAccessList)
                {
                    if (originalAction == action.TargetServiceNS || targetServiceNamespace == action.TargetServiceNS
                        || targetServiceNamespaceWithLastSlash == action.TargetServiceNS)
                    {
                        string[] roles = action.AllowedGroups.Split(new char[] { ';', ',' });
                        foreach (string role in roles)
                        {
                            bool accessAllowed = principal.IsInRole(role);
                            if (accessAllowed)
                                return true;
                        }
                    }
                }
            }

            //Check access based on Certificates
            if (thumbPrint != string.Empty)
            {
                foreach (CertAuthZElement certAccess in this.certAccessList)
                {
                    if (originalAction == certAccess.TargetServiceNS || targetServiceNamespace == certAccess.TargetServiceNS
                        || targetServiceNamespaceWithLastSlash == certAccess.TargetServiceNS)
                    {
                        string[] certs = certAccess.CertThumbprints.Split(new char[] { ';', ',' });

                        foreach (string cert in certs)
                        {
                            var allowedCert = cert.Replace(" ", string.Empty).ToUpper();
                            thumbPrint = thumbPrint.Replace(" ", string.Empty).ToUpper();

                            if (allowedCert == thumbPrint)
                                return true;
                        }
                    }
                }
            }

            return false; 
        }

        

    }
    public static class Identitytools
    {
        /// <summary>
        /// Find the thumbprint of the clients certificate
        /// </summary>
        /// <param name="operationContext"></param>
        /// <returns></returns>
        public static string GetClientCertificateThumbprint(ServiceSecurityContext securityContext)
        {
            foreach (ClaimSet claimSet in securityContext.AuthorizationContext.ClaimSets)
            {
                foreach (Claim claim in claimSet.FindClaims(ClaimTypes.Thumbprint, Rights.Identity))
                {
                    string tb = BitConverter.ToString((byte[])claim.Resource);
                    tb = tb.Replace("-", "");
                    return tb;
                }
            }

            return string.Empty;
        }
    }
}
