using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SimpleCRMs
{
    public class SampleCreateReadOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.InitiatingUserId);

            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.PrimaryEntityName == "account")
            {
                tracingService.Trace("Entered the method. Passed the test of context.PrimaryEntityName = account");
                //read
                var accountRecord = service.Retrieve("account", context.PrimaryEntityId, new ColumnSet("name", "telephone"));
                var name = accountRecord.GetAttributeValue<string>("name");
                var phoneNumber = accountRecord.GetAttributeValue<string>("phonenumber");
                var customerTypeCode = accountRecord.Contains("customertypecode") ?
                    accountRecord.GetAttributeValue<OptionSetValue>("customertypecode").Value :
                    100;

                //retreive
                tracingService.Trace($"Retreived the values from the account. Phone number: {phoneNumber} and customer type code: {customerTypeCode}, account name: {name}");

                //create
                var contactRecord = new Entity("contact");
                contactRecord["fullname"] = name;
                contactRecord["telephone"] = phoneNumber;
                contactRecord["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
                contactRecord["accountrolecode"] = new OptionSetValue(2);
                contactRecord["creditlimit"] = new Money(100);
                contactRecord["lastonholdtime"] = new DateTime(2024, 1, 1);
                contactRecord["donotphone"] = true;
                contactRecord["numberofchildren"] = 0;

                var contactGuid = service.Create(contactRecord);


                //done created
                tracingService.Trace($"All steps are executed successfully");
            }
        }
    }
}
