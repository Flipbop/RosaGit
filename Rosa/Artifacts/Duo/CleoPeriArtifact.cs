using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class CleoPeriArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi is not { } api)
			return;

		helper.Content.Artifacts.RegisterArtifact("CleoPeri", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/CleoPeri.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoPeri", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoPeri", "description"]).Localize
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.CleoDeck.Deck, Deck.peri]);
	}
	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		base.OnPlayerPlayCard(energyCost, deck, card, state, combat, handPosition, handCount);
		if (card.GetMeta().deck == Deck.peri )
		{
		}
	}
	public override bool? OnPlayerAttackMakeItPierce(State state, Combat combat)
	{
		base.OnPlayerAttackMakeItPierce(state, combat);
		if (combat.currentCardAction?.whoDidThis == Deck.peri &&
		    ModEntry.Instance.KokoroApi.ActionInfo.GetSourceCard(state, combat.currentCardAction)?.upgrade != Upgrade.None)
		{
			return true;
		}
		return false;
	}
}