using Nickel;
using System.Linq;

namespace Flipbop.Rosa;

internal static class PatientExt
{
	public static bool GetIsPatient(this Card self)
		=> ModEntry.Instance.Helper.ModData.GetModDataOrDefault<bool>(self, "Patient");

	public static void SetIsPatient(this Card self, bool value)
		=> ModEntry.Instance.Helper.ModData.SetModData(self, "Patient", value);

}

internal sealed class PatientManager
{
	internal static readonly ICardTraitEntry Trait = ModEntry.Instance.PatientTrait;
	
	public PatientManager()
	{
		int cardCount = 0;
		ModEntry.Instance.Helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnDrawCard),
			(State state, Combat combat) =>
			{
				cardCount++;
			});

		ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnTurnEnd), (State state, Combat combat) =>
		{
			for (int i = combat.discard.Count; i > 0; i--)
			{
				if (i > 0)
				{
					Card card = combat.discard[i];
					if (card.GetIsPatient() && cardCount > 0)
					{
						card.discount -= 1;
						cardCount--;
					}
				}
			}
		});
	}
}
