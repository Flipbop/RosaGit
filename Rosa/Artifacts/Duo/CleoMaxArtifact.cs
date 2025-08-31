using FSPRO;
using Nanoray.PluginManager;
using Newtonsoft.Json;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class CleoMaxArtifact : Artifact, IRegisterable
{

	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi is not { } api)
			return;

		helper.Content.Artifacts.RegisterArtifact("CleoMax", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/CleoMax.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoMax", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoMax", "description"]).Localize
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.CleoDeck.Deck, Deck.hacker]);
	}
	

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		base.OnPlayerPlayCard(energyCost, deck, card, state, combat, handPosition, handCount);
		if (card.upgrade != Upgrade.None && combat.exhausted.Contains(card))
		{
			combat.Queue(new ADrawUpgrade {Amount = 1});
		}
	}
}