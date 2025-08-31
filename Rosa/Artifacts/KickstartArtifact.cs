using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class KickstartArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Kickstart", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.CleoDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Kickstart.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Kickstart", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Kickstart", "description"]).Localize
		});
	}
	
	
	public override List<Tooltip>? GetExtraTooltips()
		=> [new GlossaryTooltip($"action.{ModEntry.Instance.Package.Manifest.UniqueName}::Improve A")
			{
				Icon = ModEntry.Instance.ImproveAIcon.Sprite,
				TitleColor = Colors.action,
				Title = ModEntry.Instance.Localizations.Localize(["action", "ImproveA", "name"]),
				Description = ModEntry.Instance.Localizations.Localize(["action", "ImproveA", "description"])
			}];

	public int Amount = 2;

	public override void OnDrawCard(State state, Combat combat, int count)
	{
		base.OnDrawCard(state, combat, count);
		if (combat.hand[^1].upgrade == Upgrade.None && combat.hand[^1].IsUpgradable() && Amount > 0)
		{
			if (state.EnumerateAllArtifacts().Any((a) => a is DailyUpgradesOnlyB))
			{
				ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(state, combat.hand[^1], ModEntry.Instance.ImprovedBTrait, true, false);
				ImprovedBExt.AddImprovedB(combat.hand[^1], state);
			}
			else
			{
				ModEntry.Instance.helper.Content.Cards.SetCardTraitOverride(state, combat.hand[^1], ModEntry.Instance.ImprovedATrait, true, false);
				ImprovedAExt.AddImprovedA(combat.hand[^1], state);
			}
			Amount--;
		}
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		base.OnCombatStart(state, combat);
		Amount = 2;
	}
	
	public override int? GetDisplayNumber(State s)
	{
		return Amount;
	}
}
