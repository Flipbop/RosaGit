using Nanoray.PluginManager;
using Nickel;

namespace Flipbop.Cleo;

internal interface IRegisterable
{
	static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}