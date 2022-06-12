using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfeoBootstrap.Install;
internal interface IInstallAction
{
    void Install();
    void Commit();
    void Rollback();
    void Uninstall();
}
