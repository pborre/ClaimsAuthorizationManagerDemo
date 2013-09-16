using System;
using System.Collections.Generic;
using System.ServiceModel.Configuration;
using System.Configuration;

namespace Riziv.BizTalk.Common
{
    public class ServiceRolesAndCertAuthZExElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            System.Diagnostics.EventLog.WriteEntry("WCF Authoriation manager", "create behavior");
            return new ServiceRolesAndCertAuthZBehavior(this.ADGroupAccessList, this.CertAccessList);
        }

        public override Type BehaviorType
        {
            get { return typeof(ServiceRolesAndCertAuthZBehavior); }
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            ServiceRolesAndCertAuthZExElement element = (ServiceRolesAndCertAuthZExElement)from;
            ADGroupAccessList = element.ADGroupAccessList;
            CertAccessList = element.CertAccessList;
        }

        [ConfigurationProperty("ADGroupAccessList", IsDefaultCollection = false),
        ConfigurationCollection(typeof(ADGroupAccessConfigurationCollection), AddItemName = "adGroupAccess", ClearItemsName = "clearAdGroupAccess", RemoveItemName = "removeAdGroupAccess")]
        public ADGroupAccessConfigurationCollection ADGroupAccessList
        {
            get { return this["ADGroupAccessList"] as ADGroupAccessConfigurationCollection; }
            set { this["ADGroupAccessList"] = value; }
        }

        [ConfigurationProperty("CertAccessList", IsDefaultCollection = false),
        ConfigurationCollection(typeof(CertAccessConfigurationCollection), AddItemName = "certAccess", ClearItemsName = "clearCertAccess", RemoveItemName = "removeCertAccess")]
        public CertAccessConfigurationCollection CertAccessList
        {
            get { return this["CertAccessList"] as CertAccessConfigurationCollection; }
            set { this["CertAccessList"] = value; }
        }
    }


    /// <summary>
    /// Class that describe what AD Groups have rights to call a certain soap action
    /// </summary>
    public class ADGroupAuthZElement : ConfigurationElement, IConfigurationElementKey
    {
        public ADGroupAuthZElement() { }
        
        public ADGroupAuthZElement(string targetServiceNS, string adGroups)
        {
            this.AllowedGroups = adGroups;
            this.TargetServiceNS = targetServiceNS;
        }

        /// <summary>
        /// Two levels will be supported:
        /// 1. target service namespace => the groups will have access to all operations of the service.
        /// 2. full soap action (=operaton) => the groups will have access to a specific operation.
        /// </summary>
        [ConfigurationProperty("targetServiceNS", IsRequired = true, DefaultValue = "<enter the target service namespace>")]
        public string TargetServiceNS
        {
            get { return (string)this["targetServiceNS"]; }
            set { this["targetServiceNS"] = value; }
        }

        [ConfigurationProperty("allowedGroups", IsRequired = true, DefaultValue = "<enter the allowed groups on the TargetServiceNS separated by a ';'")]
        public string AllowedGroups
        {
            get { return (string)this["allowedGroups"]; }
            set { this["allowedGroups"] = value; }
        }

        public override string ToString()
        {
            return KeyElement;
        }

        #region IConfigurationElementKey Members

        public string KeyElement
        {
            get { return TargetServiceNS; }
        }

        #endregion
    }

    /// <summary>
    /// Class that describes what certificates have rights to call a certain soap action
    /// </summary>
    public class CertAuthZElement : ConfigurationElement, IConfigurationElementKey
    {
        public CertAuthZElement() { }

        public CertAuthZElement(string targetServiceNS, string certThumbprints)
        {
            this.CertThumbprints = certThumbprints;
            this.TargetServiceNS = targetServiceNS;
        }

        /// <summary>
        /// Two levels will be supported:
        /// 1. target service namespace => the certificates will have access to all operations of the service.
        /// 2. full soap action (=operaton) => the certificates will have access to a specific operation.
        /// </summary>
        [ConfigurationProperty("targetServiceNS", IsRequired = true, DefaultValue = "<enter the target service namespace>")]
        public string TargetServiceNS
        {
            get { return (string)this["targetServiceNS"]; }
            set { this["targetServiceNS"] = value; }
        }

        [ConfigurationProperty("certThumbprints", IsRequired = true, DefaultValue = "<enter the allowed certificates on the TargetServiceNS separated by a ';'")]
        public string CertThumbprints
        {
            get { return (string)this["certThumbprints"]; }
            set { this["certThumbprints"] = value; }
        }

        public override string ToString()
        {
            return KeyElement;
        }

        #region IConfigurationElementKey Members

        public string KeyElement
        {
            get { return TargetServiceNS; }
        }

        #endregion
    }


    public interface IConfigurationElementKey
    {
        string KeyElement
        {
            get;
        }
    }

    public abstract class BaseConfigurationElementCollection<TConfigurationElementType> : 
        ConfigurationElementCollection, IList<TConfigurationElementType> 
        where TConfigurationElementType : ConfigurationElement, IConfigurationElementKey, new()
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TConfigurationElementType();
        } 
        protected override object GetElementKey(ConfigurationElement element)
        {
            
            return ((TConfigurationElementType)element).KeyElement;
        } 
        #region Implementation of IEnumerable<TConfigurationElementType> 
        IEnumerator<TConfigurationElementType> IEnumerable<TConfigurationElementType>.GetEnumerator()
        {
            return (IEnumerator<TConfigurationElementType>)this.GetEnumerator();
            //return this.Cast<TConfigurationElementType>().GetEnumerator();
        }

        
        #endregion 
        #region Implementation of ICollection<TConfigurationElementType> 
        public void Add(TConfigurationElementType configurationElement)
        {
            BaseAdd(configurationElement, true);
        } 
        public void Clear()
        {
            BaseClear();
        } 
        public bool Contains(TConfigurationElementType configurationElement)
        {
            return !(IndexOf(configurationElement) < 0);
        } 
        public void CopyTo(TConfigurationElementType[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        } 
        public bool Remove(TConfigurationElementType configurationElement)
        {
            BaseRemove(GetElementKey(configurationElement)); return true;
        } 
        bool ICollection<TConfigurationElementType>.IsReadOnly
        {
            get { return IsReadOnly(); }
        } 
        #endregion 
        #region Implementation of IList<TConfigurationElementType> 
        public int IndexOf(TConfigurationElementType configurationElement)
        {
            return BaseIndexOf(configurationElement);
        } 
        public void Insert(int index, TConfigurationElementType configurationElement)
        {
            BaseAdd(index, configurationElement);
        } 
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        } 
        public TConfigurationElementType this[int index]
        {
            get{return (TConfigurationElementType)BaseGet(index);}
            set{
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        } 
        #endregion
    }

    [ConfigurationCollection(typeof(ADGroupAuthZElement), AddItemName = "adGroupAccess", 
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ADGroupAccessConfigurationCollection : BaseConfigurationElementCollection<ADGroupAuthZElement>
    {
        #region Constantsprivate 
        const string CONST_ELEMENT_NAME = "adGroupAccess";
        #endregion 
        public override ConfigurationElementCollectionType CollectionType
        {
            get{return ConfigurationElementCollectionType.BasicMap;}
        } 
        protected override string ElementName
        {
            get { return CONST_ELEMENT_NAME; }
        }
    }

    [ConfigurationCollection(typeof(CertAuthZElement), AddItemName = "certAccess",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class CertAccessConfigurationCollection : BaseConfigurationElementCollection<CertAuthZElement>
    {
        #region Constantsprivate
        const string CONST_ELEMENT_NAME = "certAccess";
        #endregion
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }
        protected override string ElementName
        {
            get { return CONST_ELEMENT_NAME; }
        }
    }
    
}   


