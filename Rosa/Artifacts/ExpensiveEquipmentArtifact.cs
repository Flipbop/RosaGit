using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class ExpensiveEquipmentArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("ExpensiveEquipment", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.CleoDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/ExpensiveEquipment.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ExpensiveEquipment", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ExpensiveEquipment", "description"]).Localize
		});
	}

	public override List<Tooltip>? GetExtraTooltips()
		=> [new TTGlossary("cardtrait.discount", 1)];

	public override void OnCombatStart(State state, Combat combat)
	{
		base.OnCombatStart(state, combat);
		
		foreach (var card in state.deck)
			if (card.GetCurrentCost(state) >= 3 )
			{
				if (card.IsUpgradable() && card.upgrade == Upgrade.None)
				{
					if (state.EnumerateAllArtifacts().Any((a) => a is DailyUpgradesOnlyB))
					{
						combat.Queue([
							new AImproveBSelf {id = card.uuid},
						]);
					}
					else
					{
						combat.Queue([
							new AImproveASelf {id = card.uuid},
						]);
					}
				}
				else if (card.upgrade != Upgrade.None)
				{
					card.discount -= 1;
				}
			}
		
	}
}
