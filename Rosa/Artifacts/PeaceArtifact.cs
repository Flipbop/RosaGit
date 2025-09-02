using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class PeaceArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Peace", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.RosaDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Peace.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Peace", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Peace", "description"]).Localize
		});
	}

	public bool peace = false;

	public override void OnPlayerAttack(State state, Combat combat)
	{
		base.OnPlayerAttack(state, combat);
		peace = false;
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		base.OnTurnStart(state, combat);
		if (peace)
		{
			combat.Queue(new AEnergy(){changeAmount = 1});
		}
		peace = true;
	}
	public override void OnCombatEnd(State state)
	{
		base.OnCombatEnd(state);
		peace = false;
	}
}
