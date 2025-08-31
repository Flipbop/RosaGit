using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class EnhancedToolsArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("EnhancedTools", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.CleoDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/EnhancedTools.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "EnhancedTools", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "EnhancedTools", "description"]).Localize
		});
	}

	public bool firstCard = true;
	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		base.OnPlayerPlayCard(energyCost, deck, card, state, combat, handPosition, handCount);
		if (card.GetIsImprovedA() && firstCard)
		{
			firstCard = false;
			combat.Queue([
				new AImproveA { Amount = 1}
			]);
		}
		if (card.GetIsImprovedB() && firstCard)
		{
			firstCard = false;
			combat.Queue([
				new AImproveB { Amount = 1}
			]);
		}
	}
	
	public override void OnCombatEnd(State state)
	{
		base.OnCombatEnd(state);
		firstCard = true;
	}
}
