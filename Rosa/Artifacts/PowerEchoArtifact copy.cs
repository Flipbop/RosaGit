using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class PowerEchoArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("PowerEcho", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.CleoDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/PowerEcho.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "PowerEcho", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "PowerEcho", "description"]).Localize
		});
	}

	public bool _firstCard = true;
	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		base.OnPlayerPlayCard(energyCost, deck, card, state, combat, handPosition, handCount);
		Card newCard = card.CopyWithNewId();
		if ((card.upgrade != Upgrade.None) && _firstCard && !card.GetData(state).singleUse)
		{
			newCard.temporaryOverride = true;
			newCard.singleUseOverride = true;
			_firstCard = false;
			combat.Queue([
				new AAddCard
				{
					card = newCard, destination = CardDestination.Hand
				},
				new AImpair {Amount = 1}
			]);
		}
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		base.OnTurnStart(state, combat);
		_firstCard = true;
	}
	public override void OnCombatEnd(State state)
	{
		base.OnCombatEnd(state);
		_firstCard = true;
	}
}
