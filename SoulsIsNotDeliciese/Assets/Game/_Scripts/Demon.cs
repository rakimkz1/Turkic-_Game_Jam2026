using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Demon : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI text;
	[SerializeField] private AnimationCurve WaitCurve;
	[SerializeField] private Replices[] replices;
	[SerializeField] private Vector2 padding;
	
	public bool IsPlayingReplices = false;
	public string DefaultPos;

	private Animator anim;
	private bool isDayEnd, isFoodEnd;
	private bool isSkipping = false;
	private bool isSkipAll = false;

	private RectTransform bgRect;

	private CancellationTokenSource cts;

	private void Start()
	{
        DayManager.instance.OnNewDay += SkipAllDialogs;
		DayManager.instance.OnNewDay += PlayDayStartReplices;
		DemonKvotaManager.instance.OnKvotaFilled += PlayFoodEndReplices;

		anim = GetComponent<Animator>();
		bgRect = text.rectTransform.parent as RectTransform;
	}
    private async void PlayDayStartReplices()
	{
		if (isDayEnd) { return; }
		await UniTask.WaitUntil(()=>IsPlayingReplices == false);
		Debug.Log("Start Replice");
		isDayEnd = true;
		isFoodEnd = false;

		cts = new CancellationTokenSource();
		PlayReplices(replices[DayManager.instance.currentDay].DayStartReplices);
	}

	private void PlayFoodEndReplices()
	{
		if (isFoodEnd) { return; }
		isFoodEnd = true;
		isDayEnd = false;

		cts = new CancellationTokenSource();
		PlayReplices(replices[DayManager.instance.currentDay].FoodEndReplices);
	}

	private async UniTask PlayReplices(Speach[] replices)
	{
        isSkipAll = false;
        if (IsPlayingReplices) { return; }

		Debug.Log("Replices");
		IsPlayingReplices = true;

		for (int i = 0; i < replices.Length; i++)
		{
			Debug.Log(replices[i].Replices);
			if (replices[i].AnimPos != String.Empty)
			{
				anim.CrossFade(replices[i].AnimPos, replices[i].animCrossFades);
			}
			SetText(replices[i].Replices);

			float waitTime = Mathf.Clamp(replices[i].Replices.Length, 0, 100) * 0.1f;
			waitTime = WaitCurve.Evaluate(waitTime);
			Debug.Log($"WaitTime = {waitTime * 10}");
			await WaitReplic(waitTime*10);
		}

		anim.Play(DefaultPos);
		IsPlayingReplices = false;
	}

	private void SetText(string text)
	{
		this.text.text = TextPlayer.WrapText(text, 40);
		bgRect.sizeDelta = this.text.GetPreferredValues() + padding;
	}

	private async UniTask WaitReplic(float duration)
	{
		float time = 0;
		while (time <= duration)
		{
			time += Time.deltaTime;

			await UniTask.Yield();

			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || isSkipping || isSkipAll)
			{
				isSkipping = false;
				return;
			}
		}
	}

    private void SkipAllDialogs()
    {
		isSkipAll = true;
    }

    public void Skip() => isSkipping = true;


	[Serializable]
	public class Replices
	{
		public Speach[] DayStartReplices;
		public Speach[] FoodEndReplices;
	}
	[Serializable]
	public class Speach
	{
		public string Replices = "Default Replices";
		public string AnimPos = "DemonDefaultPose";
		public float animCrossFades = 0;
	}

	[ContextMenu("Parse")]
	public void Parse()
	{
		var dialogues = Parse1();
		Debug.Log($"Count dialogues List: {dialogues.Count}");

		Replices[] replices = new Replices[dialogues.Count / 2];

		for (int i = 0; i < dialogues.Count ; i += 2)
		{
			replices[i / 2] = new Replices();
			replices[i / 2].DayStartReplices = new Speach[dialogues[i].Length];
			replices[i / 2].FoodEndReplices = new Speach[dialogues[i + 1].Length];

			for (int j = 0; j < dialogues[i].Length; j++)
			{
				string replice = dialogues[i][j];
				replices[i / 2].DayStartReplices[j] = new Speach();
				if (replice.StartsWith("#"))
				{
					Debug.Log($"Replica: {replice}");
					var d2 = replice.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

					replices[i / 2].DayStartReplices[j].AnimPos = d2[0].Substring(1);

					replice = d2[1];
				}

				replices[i / 2].DayStartReplices[j].Replices = replice;
			}
			for (int j = 0; j < dialogues[i+1].Length; j++)
			{
				string replice = dialogues[i][j];
				replices[i / 2].FoodEndReplices[j] = new Speach();
				if (replice.StartsWith("#"))
				{
					Debug.Log($"Replica: {replice}");
					var d2 = replice.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

					replices[i / 2].FoodEndReplices[j].AnimPos = d2[0].Substring(1);

					replice = d2[1];
				}

				replices[i / 2].FoodEndReplices[j].Replices += dialogues[i + 1][j];
			}
		}

		this.replices = replices;
	}

	public List<string[]> Parse1()
	{
		string text = "#DemonAppear  \r\nWAAAH!!!\r\n\r\n#DemonDefaultPose  \r\nGood evening, Loser! Oops, my bad, I meant... night! Right!\r\n\r\n#DefaultFaceSpeech1  \r\nWhy the long face? Don't recognize this place? It's your home, Dummy!\r\n\r\n#DefaultFaceSpeech1  \r\nWhat a dump. Stop counting sheep in the sky and cook me something.\r\n\r\n#DefaultFaceSpeech3  \r\nWhy are you just standing there? You think you can talk back to me?\r\n\r\n#DefaultFaceSpeech2  \r\nHeh, I'd love to see that stubborn attitude of yours once you see your family.\r\n\r\n#ShowFamily  \r\nYep, your family's here! All seven of them. Just like those little goats from the tale. Which makes me the wolf.\r\n\r\n#JoyPose2  \r\nAlready furious, are we? Glad to hear it. Then you know what to do. I'm hungry.\r\n\r\n#DefaultFaceSpeech1  \r\n...Wait, have you seriously never cooked before?\r\n\r\n#DefaultFaceSpeech1  \r\nYou're hopeless. Looks like I'll have to teach you everything.\r\n\r\n#ShowSpirits  \r\nListen up, Loser. On the shelf to your right rest the souls of your ancestors.\r\n\r\n#ShowBoiler  \r\nEach one grants you a unique power. Pick whichever one you're willing to lose and toss it into the cauldron.\r\n\r\n#SmilePose2  \r\nAnd hurry up! I'm hungry enough to eat all seven members of your family if you keep dragging your feet!\r\n\r\n\r\n#JoyPose1  \r\nOoooh, that actually looks delicious! You're quite the cook, huh?!\r\n\r\n#JoyPose1  \r\nWhy so upset? What, suddenly feeling bad for your ancestors?\r\n\r\n#DefaultFaceSpeech2  \r\nForget about them. They've already lived their lives. Let them disappear for good! You've got your own skin to save!\r\n\r\n#SmilePose1  \r\nWell... your family's skin too, technically.\r\n\r\n#ShowCandle  \r\nAnyway, enough of that. See that candle? Blow out one of its flames.\r\n\r\n#SmilePose1  \r\nWondering what the candle's for?\r\n\r\n#JoyPose1  \r\nSomeone as dense as you could never grasp my genius, but this is one of my personal inventions.\r\n\r\n#JoyPose2  \r\nYou blow out one flame every day. Once they're all gone, the new moon will end. And so will my freedom...\r\n\r\n#DefaultFaceSpeech3  \r\n...Almost made myself cry there. Forget I said that!\r\n\r\n#ShowCandle  \r\nNow put out the damn flame so we can move on to the next day!\r\n\r\n\r\n#DefaultFaceSpeech2  \r\nGood night, Dummy!\r\n\r\n#DefaultFaceSpeech3  \r\nHow are you feeling? Doing great after yesterday, I bet.\r\n\r\nListen up. Today's been rough, and I'm starving. Cook me one of your specialties.\r\n\r\n#DefaultFaceSpeech1  \r\nAnd none of that stubborn nonsense! Got it, Loser?!\r\n\r\n\r\n#SmilePose2  \r\nOoooh, now that's delicious! No exaggeration, this deserves five Michelin stars!\r\n\r\n#JoyPose2  \r\nYou're a terrible descendant, but have you considered becoming my chef? Hahahaha!\r\n\r\n#SmilePose1  \r\nActually, never mind. You'd probably throw me into the cauldron too. You seem to have a thing for that, don't you?!\r\n\r\nWhatever. That joke wasn't funny anyway. You're pathetic. Just skip the day already.\r\n\r\n\r\n#SmilePose2  \r\nEvening, Dummy! What a beautiful night.\r\n\r\n#DefaultFaceSpeech3  \r\nLooks like you've gotten used to all this... Burning the souls of your own family!\r\n\r\n#DefaultFaceSpeech1  \r\nI heard your jezde, Kotibar Batyr, was never really a Batyr.\r\n\r\n#SmilePose1  \r\nWhat, you think I made that up? Pfff. Why would I invent nasty rumors about a man who's dead... and about to die a second time?\r\n\r\n#SmilePose1  \r\nYep. Truth is, Kotibar Batyr only pretended to be a Batyr.\r\n\r\n#DefaultFaceSpeech2  \r\nBut you know who can't pretend? You. Now go cook me a soul, and stop pretending to be a good person.\r\n\r\n\r\n#SmilePose2  \r\nPHUUUAAAH! What the hell is this garbage?!\r\n\r\n#SmilePose1  \r\nI don't think even your grandmother cooked this badly, and she made plenty of disgusting things!\r\n\r\n#DefaultFaceSpeech1  \r\nQuit staring at me like that! What, you think I'm lying?\r\n\r\n#SmilePose1  \r\nI'd bet a tooth she tried poisoning your grandfather. The funny part is, you ended up drinking the poison instead!\r\n\r\n#JoyPose2  \r\nOr do you think I'm lying about your cooking being awful?\r\n\r\nEither way, stop changing the subject. Blow out the flame and let's move on to the final day.\r\n\r\n\r\n#SmilePose1  \r\nWell, look who's here, Dummy.\r\n\r\n#DefaultFaceSpeech2  \r\nThis is our last day, so I'll keep it brief.\r\n\r\nThe new moon ends tomorrow, and my powers will disappear.\r\n\r\n#DefaultFaceSpeech1  \r\nAnd I'm not saying this because I care about you. Well... I mean... maybe I do...\r\n\r\n#SmilePose2  \r\nAnyway. Become one of us.\r\n\r\n#SmilePose1  \r\nYou're already the most messed-up person alive. You burned your own ancestors for me, a Shaitan.\r\n\r\n#DefaultFaceSpeech2  \r\nSo technically, you're not getting any worse.\r\n\r\n#DefaultFaceSpeech3  \r\nWell? Don't rush to say no. I can wait a couple more hours.\r\n\r\nWhile you're thinking, cook me something.\r\n\r\n\r\n#JoyPose1  \r\nSo? Have you decided?\r\n\r\n#SmilePose1  \r\n...\r\n\r\n#SmilePose1  \r\nWhy are you quiet? Still thinking?\r\n\r\n#DefaultFaceSpeech3  \r\nKid, I don't have much time left. So hurry it up.\r\n\r\n#DefaultFaceSpeech2  \r\nYou did burn your ancestors, sure. But let's be honest, they weren't exactly angels either.\r\n\r\n#JoyPose2  \r\nOne was a womanizer, another a liar, and another a fraud. What, you think you're the only one carrying sins?\r\n\r\n#DefaultFaceSpeech1  \r\nBut none of them can become a Shaitan. They're all hypocrites who can't admit their own faults.\r\n\r\n#DefaultFaceSpeech2  \r\nYou can. That's why I'm offering you a place beside me.\r\n\r\n#SmilePose1  \r\nWell?\r\n\r\n#SmilePose2  \r\nAAARGH!!! If you're not going to answer, I'll come back next new moon!\r\n\r\n#DefaultFaceSpeech3  \r\nI'll be waiting for your answer! And that answer better start with a \"Y\" and end with an \"S\", got it?!\r\n\r\n#DefaultFaceSpeech3  \r\nDon't disappoint me, kid!";

		Debug.Log(text);

		string[] dialogueBlocks =
			text.Split("\r\n\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);


		List<string[]> dialogues = new();

		foreach (string block in dialogueBlocks)
		{
			string[] lines =
				block.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);

			dialogues.Add(lines);
		}

		return dialogues;
	}


}
