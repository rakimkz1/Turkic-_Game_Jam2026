using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins.Core.PathCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

	private RectTransform bgRect;

	private CancellationTokenSource cts;

	private void Start()
	{
		DayManager.instance.OnNewDay += PlayDayStartReplices;
		DemonKvotaManager.instance.OnKvotaFilled += PlayFoodEndReplices;

		anim = GetComponent<Animator>();
		bgRect = text.rectTransform.parent as RectTransform;
	}

	private void PlayDayStartReplices()
	{
		if (isDayEnd) { return; }
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

			if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || isSkipping)
			{
				isSkipping = false;
				return;
			}
		}
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
				replices[i / 2].FoodEndReplices[j] = new Speach();
				replices[i / 2].FoodEndReplices[j].Replices += dialogues[i + 1][j];
			}
		}

		this.replices = replices;
	}

	public List<string[]> Parse1()
	{
		string text = "#DemonAppear\r\nВаа!!!\r\n\r\n#DemonDefaultPose\r\nДоброе утро, Лошара! Ой, я ошибся, я имел ввиду... Ночь! Да!\r\n\r\n#DefaultFaceSpeech1\r\nЧего хмурый такой, не узнаешь это место? Это же твой дом, Балбес!\r\n\r\n#DefaultFaceSpeech1\r\nНу и дерьмо. Хватит считать овец на небе и приготовь мне что ни будь.\r\n\r\n#DefaultFaceSpeech3\r\nИ чего стоишь? Думаешь можешь мне перечить? \r\n\r\n#DefaultFaceSpeech2\r\nХах, я бы посмотрел на твою несгибаемость когда ты увидишь свою семью.\r\n\r\n#ShowFamily \r\nДа-да, твоя семья тут! Все семеро, как те козлята. А я получается волк.\r\n\r\n#JoyPose2\r\nО, уже рвешься и мечешь? Рад слышать, тогда ты знаешь что делать, я проголодался.\r\n\r\n#DefaultFaceSpeech1\r\n... Ты что, никогда в жизни не готовил?\r\n\r\n#DefaultFaceSpeech1\r\nВот же ж бестолочь, придется тебя всему обучать.\r\n\r\n#ShowSpirits\r\nИ так, слушай сюда, Лошара, справа от тебя находится полка, где покоятся твои предки.\r\n\r\n#ShowBoiler \r\nКаждый из них дает тебе силу, уникальную. Выбери ту которую не жалко и закидывай в котел.\r\n\r\n#SmilePose2\r\nИ побыстрее! Я голоден и готов съесть семерых твоих семьи, если ты не поторопишься!\r\n\r\n\r\n#JoyPose1\r\nУуухх, вышло очень даже аппетитно! Из тебя хороший повар, А?!\r\n\r\n#JoyPose1\r\nИ чего ты так расстроился? Че, предков жалко стало? \r\n\r\n#DefaultFaceSpeech2 \r\nДа забей ты на них. Они свое уже прожили, и пускай уже окончательно сгинут! А тебе надо свою шкуру спасать!\r\n\r\n#SmilePose1\r\nНу... Свою семью, да.\r\n\r\n#ShowCandle\r\nЛадно, проехали. Видишь ту свечку? Сдуй ка одну из его пламеней.\r\n\r\n#SmilePose1\r\nИнтересно стало что за свеча? \r\n\r\n#JoyPose1\r\nНу, такому тупому как ты, сложно будет дотянуться до моих гениальных мыслей, но это моя личная разработка. \r\n\r\n#JoyPose2\r\nКаждый день надо сдувать по одной и когда все пламени угаснут, это будет означать конец новолунию. А значит и конец моей свободе...\r\n\r\n#DefaultFaceSpeech3\r\n... Аж на слезу пробило... Отставить!\r\n\r\n#ShowCandle\r\nПотуши ты эту гребанную свечку и пойдем уже в следующий день!\r\n\r\n\r\n#DefaultFaceSpeech2 \r\nДоброй ночи, Лошок!\r\n\r\n#DefaultFaceSpeech3\r\nКак дела? Как ощущения? Отлично себя чувствуешь после вчерашнего? А то!\r\n\r\nТак, слушай сюда, сегодня у меня трудный денек выдался и я кушать хочу. Приготовь ка мне одного из своих деликатесов!\r\n\r\n#DefaultFaceSpeech1\r\nИ чтобы без этого твоего упрямства! Понял, Лошара?!\r\n\r\n\r\n#SmilePose2\r\nОоо какая вкуснотень! Это без преувеличений достойно пяти звезд Мишлена!\r\n\r\n#JoyPose2\r\nИз тебя потомок такой себе, не думал стать повором у меня? Ахахахаах\r\n\r\n#SmilePose1\r\nХотя нет, ведь так ты и меня в котел запихаешь, ведь у тебя к этому какая та личная тяга! Разве нет?!\r\n\r\nНу и ладно, не смешно уже стало. Какой же ты жалкий! Пропусти уже день.\r\n\r\n\r\n#SmilePose2\r\nНочь, Лошок! Какая же прекрасная ночь. \r\n\r\n#DefaultFaceSpeech3\r\nСмотрю ты уже привык ко всему этому... Сжиганию своих родственных тебе душ!\r\n\r\n#DefaultFaceSpeech1\r\nЯ тут слышал что твой жезде, Котибар Батыр никогда и не был Батыром.\r\n\r\n#SmilePose1\r\nЧего, думаешь я это сам придумал? Пффф, зачем мне что то гадкое придумывать о человеке, который умер, и который умрет во второй раз?\r\n\r\n#SmilePose1\r\nДа. Правда в том, что Котибар Батыр только притворялся Батыром.\r\n\r\n#DefaultFaceSpeech2 \r\nНо кто не может притворяться, так это ты. Иди приготовь мне одну душу, и не притворяйся хорошим.\r\n\r\n\r\n#SmilePose2\r\nПХУУАааа! Ну и дерьмище! \r\n\r\n#SmilePose1\r\nДумаю даже твоя бабка так отвратительно не готовила, а она готовила много мерзкого!\r\n\r\n#DefaultFaceSpeech1\r\nУбери свой презренный взгляд! Ты че, хочешь сказать что я обманываю?\r\n\r\n#SmilePose1\r\nДа я зуб ставлю, что она пыталась отравить твою дедушку. Хотя по итогу этот яд выпил ты!\r\n\r\n#JoyPose2\r\nА, или ты думаешь что я вру что ты плохо готовишь?\r\n\r\nВ любом случая, ты мне зубы не заговаривай, потуши пламя, проехали на последний день.\r\n\r\n\r\n#SmilePose1\r\nНу здарова, Лошок!\r\n\r\n#DefaultFaceSpeech2 \r\nЭто наш последний день, так что будет предельно краток.\r\n\r\nНоволуние завтра закончится и мои силы исчезнут. \r\n\r\n#DefaultFaceSpeech1\r\nЭто я говорю не потому что забочусь о тебе, ну... Как бы да... Забочусь...\r\n\r\n#SmilePose2\r\nВ общем. Стань одним из нас. \r\n\r\n#SmilePose1\r\nТы и так уже самый конченный человек на свете который сжег своих предков ради меня, Шайтана.\r\n\r\n#DefaultFaceSpeech2 \r\nТак что по сути ты не становишься хуже. \r\n\r\n#DefaultFaceSpeech3\r\nНу че? Ты только не спеши отказываться, я могу подождать пару часиков\r\n\r\nПока думаешь, приготовь что ни будь.\r\n\r\n\r\n#JoyPose1\r\nНу что подумал?\r\n\r\n#SmilePose1\r\n...\r\n\r\n#SmilePose1\r\nЧе молчишь? Неужто еще думаешь?\r\n\r\n#DefaultFaceSpeech3\r\nМалой, у меня не так уж и много времени. Так что давай быстрее.\r\n\r\n#DefaultFaceSpeech2 \r\nТы конечно и сжег своих предков, но и они, скажем честно, не были ангелами.\r\n\r\n#JoyPose2\r\nОдин вот бабник, другой лжец, и еще один, самозванец. Че, думаешь один ты имеешь грех на плечу?\r\n\r\n#DefaultFaceSpeech1\r\nНо не все они могут стать Шайтанам, ибо все они поголовно лицемеры и не могут признаться в своих грехах.\r\n\r\n#DefaultFaceSpeech2 \r\nНо а ты можешь, потому и предлагаю тебе стать подобным мне.\r\n\r\n#SmilePose1\r\nНу че? \r\n\r\n#SmilePose2\r\nАААГХ!!! раз не собираешься отвечать, я приду в следующее новолуние!\r\n\r\n#DefaultFaceSpeech3\r\nБуду ждать от тебя ответа! И чтобы этот ответ начинался на \"Д\" и заканчивался на \"А\", Понял?!\r\n\r\n#DefaultFaceSpeech3\r\nНе расстраивай меня, малой!";

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
