using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCRMs
{
    public class SampleUpdateOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.InitiatingUserId);


            if (context.PrimaryEntityName == "account")
            {
                var accountRecord = service.Retrieve("account", context.PrimaryEntityId, new ColumnSet("accountratingcode", "numberofemployees"));

                int accountRating = accountRecord.Contains("accountratingcode") ? accountRecord.GetAttributeValue<OptionSetValue>("accountratingcode").Value : 100;

                int numberOfEmployees = accountRecord.Contains("numberofemployees") ? accountRecord.GetAttributeValue<int>("numberofemployees") : 0;

                var accountToUpdate = new Entity("account");
                accountToUpdate.Id = context.PrimaryEntityId;
                accountToUpdate["revenue"] = (accountRating == 1 && numberOfEmployees < 10) ? new Money(50) : new Money(100);
                service.Update(accountToUpdate);

                //delete an account
                //service.Delete("account", context.PrimaryEntityId);
            }

        }
    }
}
