using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCRMs
{
    public class SampleSelectOperation : IPlugin
    {

        public const string GetContactRecords = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' savedqueryid='00000000-0000-0000-00aa-000010001003' no-lock='false' distinct='true'>
                              <entity name='contact'>
                                <attribute name='entityimage_url'/>
                                <attribute name='fullname'/>
                                <order attribute='fullname' descending='false'/>
                                <attribute name='parentcustomerid'/>
                                <attribute name='telephone1'/>
                                <attribute name='emailaddress1'/>
                                <attribute name='contactid'/>
                                <filter type='and'>
                                  <condition attribute='parentcustomerid' operator='eq' value='{0}' uitype='account'/>
                                </filter>
                              </entity>
                            </fetch>";

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.InitiatingUserId);

            if (context.PrimaryEntityName == "contact")
            {
                var contactQe = new QueryExpression("contact");
                contactQe.ColumnSet = new ColumnSet("fullname", "telephone1", "parentcustomerid", "creditlimit");
                contactQe.Criteria.AddCondition(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, context.PrimaryEntityId));

                var contactRecords = service.RetrieveMultiple(contactQe);

                var queryXML = string.Format(GetContactRecords, context.PrimaryEntityId);
                var fetchXML = new FetchExpression(queryXML);
                var contactRecordsFromFetchingXML = service.RetrieveMultiple(fetchXML);

            }
        }
    }
}
