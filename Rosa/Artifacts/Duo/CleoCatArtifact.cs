using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace Flipbop.Cleo;

internal sealed class CleoCatArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		if (ModEntry.Instance.DuoArtifactsApi is not { } api)
			return;

		helper.Content.Artifacts.RegisterArtifact("CleoCat", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/CleoCat.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoCat", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoCat", "description"]).Localize
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.CleoDeck.Deck, Deck.colorless]);
	}

	public override void OnReceiveArtifact(State state)
	{
		base.OnReceiveArtifact(state);
		state.GetCurrentQueue().Add(new AAddCard { amount = 1, card = new CannonColorless {upgrade = Upgrade.A}});
		state.GetCurrentQueue().Add(new AAddCard { amount = 1, card = new BasicShieldColorless {upgrade = Upgrade.A}}); 
		state.GetCurrentQueue().Add(new AAddCard { amount = 1, card = new DodgeColorless() {upgrade = Upgrade.A}}); 
	}
}