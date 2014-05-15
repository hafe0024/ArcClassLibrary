using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.PLM
{
    [Serializable]
    public class RowInfo
    {

        public string locationTract;
        public string locationDistrict;
        public string locationLineSection;
        public string locationState;
        public string locationCountry;
        public string locationLegalDescription;
        public string locationSection;
        public string locationTownship;
        public string locationRange;

        public string tenantName;
        public string tenantMainPhone;
        public string tenantSecondaryPhone;
        public string tenantCountry;
        public string tenantRowComments;
        public bool? tenantContactMade;

        public string accessHowGained;
        public bool? accessRoutePreapproved;
        public double? accessLength;
        public double? accessWidth;
        public bool? accessDamagedWhileAcessing;
        public bool? accessRestorationCompleted;
        public bool? accessRestorationApprovedByOwner;

        public double? workAreaLength;
        public double? workAreaWidth;
        public bool? workAreaRestorationCompleted;
        public bool? workAreaRestorationApproved;
        public bool? workAreaPesticidesInLast72Hours;





        public RowInfo()
        {

        }

        public void setLocationTabValues(string tract, string district, string lineSection, Object state, Object country, string legalDescription,
            string section, string township, string range)
        {
            this.locationTract = tract;
            this.locationDistrict = district;
            this.locationLineSection = lineSection;
            this.locationState = PLM_Helpers.getComboBoxSelectedValue(state);
            this.locationCountry = PLM_Helpers.getComboBoxSelectedValue(country);
            this.locationLegalDescription = legalDescription;
            this.locationSection = section;
            this.locationTownship = township;
            this.locationRange = range;
        }

        public void setTenantTabValues(string tenantName, string tenantMainPhone, string tenantSecondaryPhone, Object tenantCountry,
            string tenantRowComments, Object tenantContactMade)
        {
            this.tenantName = tenantName;
            this.tenantMainPhone = tenantMainPhone;
            this.tenantSecondaryPhone = tenantSecondaryPhone;
            this.tenantCountry = PLM_Helpers.getComboBoxSelectedValue(tenantCountry);
            this.tenantRowComments = tenantRowComments;
            this.tenantContactMade = PLM_Helpers.trueFalseValue(tenantContactMade);
        }

        public void setAccessTabValues(Object accessHowGained, Object accessRoutePreapproved, string accessLength, string accessWidth,
            Object accessDamagedWhileAcessing, Object accessRestorationCompleted, Object accessRestorationApprovedByOwner)
        {
            this.accessHowGained = PLM_Helpers.getComboBoxSelectedValue(accessHowGained);
            this.accessRoutePreapproved = PLM_Helpers.trueFalseValue(accessRoutePreapproved);
            this.accessLength = PLM_Helpers.convertStringToNullableDouble(accessLength);
            this.accessWidth = PLM_Helpers.convertStringToNullableDouble(accessWidth);
            this.accessDamagedWhileAcessing = PLM_Helpers.trueFalseValue(accessDamagedWhileAcessing);
            this.accessRestorationCompleted = PLM_Helpers.trueFalseValue(accessRestorationCompleted);
            this.accessRestorationApprovedByOwner = PLM_Helpers.trueFalseValue(accessRestorationApprovedByOwner);
        }

        public void setWorkAreaTabValues(string workAreaLength, string workAreaWidth, Object workAreaRestorationCompleted,
            Object workAreaRestorationApproved, Object workAreaPesticidesInLast72Hours)
        {
            this.workAreaLength = PLM_Helpers.convertStringToNullableDouble(workAreaLength);
            this.workAreaWidth = PLM_Helpers.convertStringToNullableDouble(workAreaWidth);
            this.workAreaRestorationCompleted = PLM_Helpers.trueFalseValue(workAreaRestorationCompleted);
            this.workAreaRestorationApproved = PLM_Helpers.trueFalseValue(workAreaRestorationApproved);
            this.workAreaPesticidesInLast72Hours = PLM_Helpers.trueFalseValue(workAreaPesticidesInLast72Hours);
        }

    }
}
