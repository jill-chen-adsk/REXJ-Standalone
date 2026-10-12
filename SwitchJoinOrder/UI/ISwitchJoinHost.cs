using System.Collections.Generic;
using ADSK.JExtRAC.SwitchJoinOrder.Entities;

namespace ADSK.JExtRAC.SwitchJoinOrder.UI
{
    public interface ISwitchJoinHost
    {
        List<string> ListValueSetting { get; }
        void OpenFamilyDetail(CategoryItem categoryItem);
    }
}
