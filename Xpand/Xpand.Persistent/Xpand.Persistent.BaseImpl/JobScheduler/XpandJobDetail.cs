﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Quartz;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {

    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("JobDetail")]
    public class XpandJobDetail : XpandCustomObject, IJobDetail {
        public XpandJobDetail(Session session)
            : base(session) {
        }
        private string _name;
        [RuleRequiredField]
        [RuleUniqueValue(null, DefaultContexts.Save)]
        public string Name {
            get {
                return _name;
            }
            set {
                SetPropertyValue("Name", ref _name, value);
            }
        }
        [Tooltip("Whether or not the IJob implements the interface IStatefulJob.")]
        public virtual bool Stateful {
            get {
                if (_jobType == null) {
                    return false;
                }
                return (XafTypesInfo.Instance.FindTypeInfo(typeof(IStatefulJob)).Type.IsAssignableFrom(_jobType));
            }
        }

        private string _group;
        [Tooltip(@"Get or sets the group of this IJob. If null, ""DEFAULT"" will be used. ")]
        public string Group {
            get {
                return _group;
            }
            set {
                SetPropertyValue("Group", ref _group, value);
            }
        }
        private string _description;
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get {
                return _description;
            }
            set {
                SetPropertyValue("Description", ref _description, value);
            }
        }
        private Type _jobType;
        [RuleRequiredField]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(JobTypeClassInfoConverter))]
        public Type JobType {
            get {
                return _jobType;
            }
            set {
                SetPropertyValue("JobType", ref _jobType, value);
            }
        }
        private XpandJobDataMap _jobDataMap;
        [Browsable(false)]
        public XpandJobDataMap JobDataMap {
            get {
                return _jobDataMap;
            }
            set {
                SetPropertyValue("JobDataMap", ref _jobDataMap, value);
            }
        }
        private bool _requestsRecovery;
        [Tooltip("Whether or not the the IScheduler should re-Execute the IJob if a 'recovery' or 'fail-over' situation is encountered.")]
        public bool RequestsRecovery {
            get {
                return _requestsRecovery;
            }
            set {
                SetPropertyValue("RequestsRecovery", ref _requestsRecovery, value);
            }
        }
        private bool _volatile;
        [Tooltip("Whether or not the IJob should not be persisted in the IJobStore for re-use after program restarts. If not explicitly set, the default value is false. ")]
        public bool Volatile {
            get {
                return _volatile;
            }
            set {
                SetPropertyValue("Volatile", ref _volatile, value);
            }
        }
        private bool _durable;
        [Tooltip("Whether or not the IJob should remain stored after it is orphaned (no Triggers point to it). If not explicitly set, the default value is false. ")]
        public bool Durable {
            get {
                return _durable;
            }
            set {
                SetPropertyValue("Durable", ref _durable, value);
            }
        }
        [Association("JobDetailTriggerLink-Triggers")]
        protected IList<JobDetailTriggerLink> TriggerLinks {
            get {
                return GetList<JobDetailTriggerLink>("TriggerLinks");
            }
        }
        [ManyToManyAlias("TriggerLinks", "JobTrigger")]
        public IList<XpandJobTrigger> JobTriggers {
            get { return GetList<XpandJobTrigger>("JobTriggers"); }
        }
        IList<IJobTrigger> IJobDetail.JobTriggers {
            get {
                return new ListConverter<IJobTrigger, XpandJobTrigger>(JobTriggers);
            }
        }
        [Association("JobDetailJobListenerTriggerLink-JobListeners")]
        protected IList<JobDetailJobListenerTriggerLink> JobListenerTriggerLinks {
            get {
                return GetList<JobDetailJobListenerTriggerLink>("JobListenerTriggerLinks");
            }
        }
        [ManyToManyAlias("JobListenerTriggerLinks", "JobListenerTrigger")]
        public IList<JobListenerTrigger> JobListenerTriggers {
            get { return GetList<JobListenerTrigger>("JobListenerTriggers"); }
        }
        IList<IJobListenerTrigger> IJobDetail.JobListenerTriggers {
            get {
                return new ListConverter<IJobListenerTrigger, JobListenerTrigger>(JobListenerTriggers);
            }
        }
    }
}
