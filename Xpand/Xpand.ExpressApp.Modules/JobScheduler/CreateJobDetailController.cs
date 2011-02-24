﻿using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using Quartz;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class CreateJobDetailController : SupportSchedulerController {
        JobDetail _currentJobDetail;

        public CreateJobDetailController() {
            TargetObjectType = typeof(IJobDetail);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleted += ObjectSpaceOnObjectDeleted;
            View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleted -= ObjectSpaceOnObjectDeleted;
            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
        }

        void ViewOnCurrentObjectChanged(object sender, EventArgs eventArgs) {
            IScheduler scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            var detail = ((IJobDetail)View.CurrentObject);
            _currentJobDetail = scheduler.GetJobDetail(detail.Name, detail.Group);
        }

        void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            IScheduler scheduler = Application.FindModule<JobSchedulerModule>().Scheduler;
            objectsManipulatingEventArgs.Objects.OfType<IJobDetail>().ToList().ForEach(detail => scheduler.DeleteJob(detail.Name, detail.Group));
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            var xpandJobDetail = ((IJobDetail)View.CurrentObject);
            ObjectSpace.GetNewObjectsToSave<IJobDetail>().ToList().ForEach(AddJob);
            ObjectSpace.GetObjectsToUdate<IJobDetail>().ToList().ForEach(UpdateJob);
            _currentJobDetail = Scheduler.GetJobDetail(xpandJobDetail.Name, xpandJobDetail.Group);
        }

        void UpdateJob(IJobDetail xpandJobDetail) {
            var triggers = Scheduler.GetTriggersOfJob(_currentJobDetail.Name, _currentJobDetail.Group).ToList();
            Scheduler.DeleteJob(_currentJobDetail.Name, _currentJobDetail.Group);
            AddJob(xpandJobDetail);
            foreach (var trigger in triggers) {
                trigger.JobGroup = xpandJobDetail.Group;
                Scheduler.ScheduleJob(trigger);
            }
        }

        void AddJob(IJobDetail xpandJobDetail) {
            JobDetail jobDetail = Mapper.GetJobDetail(xpandJobDetail);
            jobDetail.Durable = true;
            Scheduler.AddJob(jobDetail, false);
        }
    }
}