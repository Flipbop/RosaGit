using Nanoray.PluginManager;
using Nickel;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class ReusableMaterialsArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("ReusableMaterials", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.CleoDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/ReusableMaterials.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ReusableMaterials", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "ReusableMaterials", "description"]).Localize
		});
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		base.OnPlayerPlayCard(energyCost, deck, card, state, combat, handPosition, handCount);
		if (card.GetIsImprovedA() || card.GetIsImprovedB())
		{
			combat.Queue([
				new AStatus { targetPlayer = true, status = Status.shield, statusAmount = 1 }
			]);
		}
	}
}
