using Nanoray.PluginManager;
using Newtonsoft.Json;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class CleoBooksArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi is not { } api)
			return;

		
		helper.Content.Artifacts.RegisterArtifact("CleoBooks", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/CleoBooks.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoBooks", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoBooks", "description"]).Localize
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.CleoDeck.Deck, Deck.shard]);
	}

	public override List<Tooltip>? GetExtraTooltips()
		=> [new TTCard() {card = new ShardCard()},
			ModEntry.Instance.Api.GetImprovedATooltip(true)
		];

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		base.OnPlayerPlayCard(energyCost, deck, card, state, combat, handPosition, handCount);
		if (card is ShardCard)
		{
			if (state.deck[^1].upgrade == Upgrade.None && state.deck[^1].IsUpgradable())
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
			}
		}
	}
}