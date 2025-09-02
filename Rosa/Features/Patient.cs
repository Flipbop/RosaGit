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
		ModEntry.Instance.Helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnTurnEnd), (State state, Combat combat) =>
		{
			foreach (Card card in combat.discard)
			{
				if (card.GetIsPatient() && !ModEntry.Instance.Helper.ModData.GetModDataOrDefault<bool>(card, "PatientTick")) 
				{ 
					card.discount -= 1; 
					ModEntry.Instance.Helper.ModData.SetModData(card, "PatientTick", true);
				}
			}
			foreach (Card card in combat.hand)
			{
				if (card.GetIsPatient()) 
				{ 
					card.discount -= 1; 
					ModEntry.Instance.Helper.ModData.SetModData(card, "PatientTick", true);
				}
			}
		});
		ModEntry.Instance.helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnDrawCard),
			(State state, Combat combat, int count) =>
			{
				foreach (Card card in combat.hand) ModEntry.Instance.Helper.ModData.SetModData(card, "PatientTick", false);
			});
		ModEntry.Instance.helper.Events.RegisterAfterArtifactsHook(nameof(Artifact.OnCombatEnd),
			(State state) =>
			{
				foreach (Card card in state.deck) ModEntry.Instance.Helper.ModData.SetModData(card, "PatientTick", false);
			});
	}
}
