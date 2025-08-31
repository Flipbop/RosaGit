using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using System.Linq;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class UpgradedTerminalArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("UpgradedTerminal", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.CleoDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/UpgradedTerminal.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "UpgradedTerminal", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "UpgradedTerminal", "description"]).Localize
		});
	}
	public bool _used = false;
	public override void OnDrawCard(State state, Combat combat, int count)
	{
		base.OnDrawCard(state, combat, count);
		int index = combat.hand.Count -1-count;
		int upgradeCount = 0;
		
		while (index >= 0)
		{
			if (combat.hand[index].upgrade != Upgrade.None)
			{
				upgradeCount++;
			} 
			index--;
		}
		if (upgradeCount >= 3 && !_used)
		{
			_used = true;
			combat.Queue([
				new ADrawCard {count = 1}
			]);
		}
	}
	public override void OnTurnEnd(State state, Combat combat)
	{
		base.OnTurnEnd(state, combat);
		_used = false;
	}
}
