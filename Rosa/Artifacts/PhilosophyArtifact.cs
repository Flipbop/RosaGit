using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class PhilosophyArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Philosophy", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.RosaDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Philosophy.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Philosophy", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Philosophy", "description"]).Localize
		});
	}
	public override List<Tooltip>? GetExtraTooltips()
		=> [new GlossaryTooltip($"action.{ModEntry.Instance.Package.Manifest.UniqueName}::Patient")
		{
			Icon = ModEntry.Instance.PatientIcon.Sprite,
			TitleColor = Colors.action,
			Title = ModEntry.Instance.Localizations.Localize(["cardTrait", "Patient", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["cardTrait", "Patient", "description"])
		}];

	public override void OnCombatStart(State state, Combat combat)
	{
		base.OnCombatStart(state, combat);
		foreach (var card in state.deck)
		{
			if (card.GetCurrentCost(state) >= 3 )
			{
				card.SetIsPatient(true);
			}
		}
	}
	public override void OnCombatEnd(State state)
	{
		base.OnCombatEnd(state);
		foreach (var card in state.deck)
		{
			if (card.GetCurrentCost(state) >= 3 )
			{
				card.SetIsPatient(false);
			}
		}
	}
}
