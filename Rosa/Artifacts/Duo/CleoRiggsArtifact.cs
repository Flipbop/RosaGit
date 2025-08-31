using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class CleoRiggsArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi is not { } api)
			return;

		helper.Content.Artifacts.RegisterArtifact("CleoRiggs", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/CleoRiggs.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoRiggs", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoRiggs", "description"]).Localize
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.CleoDeck.Deck, Deck.riggs]);
	}

	public override List<Tooltip>? GetExtraTooltips()
		=> StatusMeta.GetTooltips(Status.evade, 1);

	public static int UpgradeCount = 0;
	public override void OnDrawCard(State state, Combat combat, int count)
	{
		int index = combat.hand.Count-1;
		int counter = 0;
		while (counter <= count && index >= 0)
		{
			if (combat.hand[index].upgrade != Upgrade.None)
			{
				UpgradeCount++;
				if (UpgradeCount <= 3)
				{
					combat.Queue(new AStatus {status = Status.evade, statusAmount = 1, targetPlayer = true});
					UpgradeCount = 0;
				}
			}
			index--;
			counter++;
		}
		
	}

	public override int? GetDisplayNumber(State s)
	{
		return UpgradeCount;
	}

}