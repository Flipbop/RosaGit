using Nanoray.PluginManager;
using Nickel;

namespace Flipbop.Rosa;

internal interface IRegisterable
{
	static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}