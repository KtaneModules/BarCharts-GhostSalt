using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using UnityEngine.UI;
using Assets;

public class BarChartsScript : MonoBehaviour
{
    static int _moduleIdCounter = 1;
    int _moduleID = 0;

    public KMBombModule Module;
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] Selectables;
    public Image[] Highlights;
    public Image[] Bases;
    public Image[] Colourings;
    public Text UnitRend;
    public Text[] BarTextRends;
    public MeshRenderer ModuleBG;

    public KMRuleSeedable RuleSeed;

    private const float RandomTolerance = 0.45f;
    private static readonly VariableSet[] AllVariableSets = new[]
    {
        new VariableSet("Vehicles", new[]{ "Trains", "Lorries", "Cars", "Boats", "Planes", "Buses", "Bikes", "Tractors" }),
        new VariableSet("Currencies", new[]{ "Rand", "Rouble", "Dollar", "Peso", "Dinar", "Krone", "Rupee", "Pound", "Franc", "Yen", "Euro", "Lira" }),
        new VariableSet("Dwarf Planets", new[]{ "Eris", "Makemake", "Sedna", "Haumea", "Pluto", "Ceres", "Pallas", "Quaoar" }),
        new VariableSet("Cast of Star Trek: TOS", new[]{ "Spock", "McCoy", "Scott", "Chekov", "Uhura", "Kirk", "Sulu" }),
        new VariableSet("Cast of BFDI", new[]{ "Golf Ball", "Coiny", "Pin", "Ice Cube", "Spongy", "David", "Woody", "Snowball", "Pen", "Pencil", "Bubble", "Teardrop", "Eraser", "Flower", "Needle", "Rocky", "Firey", "Match", "Blocky", "Leafy", "Tennis Ball" }),
        new VariableSet("Major Websites", new[]{ "Reddit", "Instagram", "YouTube", "Craigslist", "Google", "Facebook", "Amazon", "Twitter", "Wikipedia", "eBay" }),
        new VariableSet("Esolangs", new[]{ "Funciton", "Shakespeare", "Brainf---", "Malbolge", "Piet", "Befunge", "Chef", "Intercal" }),
        new VariableSet("Basic Shapes", new[]{ "Capsule", "Octahedron", "Cylinder", "Cube", "Cone", "Torus", "Prism", "Sphere" }),
        new VariableSet("Four-Letter Countries", new[]{ "Togo", "Peru", "Iraq", "Chad", "Laos", "Mali", "Oman", "Fiji", "Cuba", "Iran", "Niue" }),
        new VariableSet("European Capital Cities", new[]{ "Budapest", "Prague", "Stockholm", "Rome", "Paris", "Lisbon", "Minsk", "Vienna", "Moscow", "Madrid", "Bucharest", "London", "Kyiv", "Warsaw", "Brussels", "Glasgow", "Riga", "Berlin", "Athens" }),
        new VariableSet("Coffee Types", new[]{ "Espresso", "Cappuccino", "Doppio", "Macchiato", "Flat White", "Latte", "Americano", "Cortado", "Mocha" }),
        new VariableSet("Fruits", new[]{ "Lemon", "Grape", "Banana", "Cherry", "Orange", "Mango", "Apple", "Pineapple", "Watermelon", "Strawberry", "Kiwi" }),
        new VariableSet("Pets", new[]{ "Hamster", "Ferret", "Bird", "Dog", "Tortoise", "Snake", "Cat", "Horse", "Spider", "Fish", "Rabbit" }),
        new VariableSet("Zodiac Signs", new[]{ "Capricorn", "Gemini", "Taurus", "Aquarius", "Sagittarius", "Leo", "Pisces", "Cancer", "Virgo", "Scorpio", "Libra", "Aries" }),
        new VariableSet("Vanilla Modules", new[]{ "Morse Code", "Knobs", "Simon Says", "The Button", "C. Discharge", "Wires", "Wire Seqs.", "Passwords", "Venting Gas", "Memory", "Mazes", "Cmp. Wires", "Keypads", "WOF" }),
        new VariableSet("Non-Percussion Instruments", new[]{ "Viola", "Euphonium", "Trumpet", "Cor Anglais", "Bassoon", "Clarinet", "Trombone", "Flugelhorn", "Piccolo", "Harp", "French Horn", "Flute", "Oboe", "Tuba", "Violin", "Double Bass", "Cello", "Saxophone", "Cornet" }),
        new VariableSet("Percussion Instruments", new[]{ "Tambourine", "Bass Drum", "Vibraphone", "Gong", "Glockenspiel", "Snare Drum", "Timpani", "Xylophone", "Marimba", "Triangle", "Cymbals" }),
        new VariableSet("C# Data Types", new[]{ "Character", "String", "Integer", "Long", "Boolean", "Double", "Float" }),
        new VariableSet("Biscuits", new[]{ "Bourbon", "Party Ring", "Ginger Nut", "Finger", "Oreo", "Rich Tea", "Nice", "Hobnobs", "Choc. Chip", "Garibaldi", "Shortbread", "Malted Milk", "Digestive" }),
        new VariableSet("James Bond Actors", new[]{ "Craig", "Brosnan", "Connery", "Lazenby", "Dalton", "Moore", "Niven" }),
        new VariableSet("Rennaissance Composers", new[]{ "Lasso", "Tallis", "Byrd", "Monteverdi", "Taverner", "Janequin", "Palestrina", "Gesualdo", "Kirbye", "Prez", "Vautor", "Morley", "Gibbons", "Weelkes" }),
        new VariableSet("20th Century Composers", new[]{ "Copland", "Bernstein", "Prokofiev", "Williams", "Cage", "Holst", "Schoenberg", "Debussy", "Bartók", "Elgar", "Gershwin", "Ravel", "Stravinsky", "Britten", "Strauss" })
    };

    private VariableSet[] VariableSets;

    private static readonly Dictionary<string, string> Deabbreviator = new Dictionary<string, string>
    {
        { "C. Discharge", "Capacitor Discharge" },
        { "Wire Seqs.", "Wire Sequences" },
        { "Cmp. Wires", "Complicated Wires" },
        { "WOF", "Who's on First" },
        { "Choc. Chip", "Chocolate Chip" }
    };

    private Dictionary<BarColor, int> ColorValues;
    private BarRule[] BarRules;
    private YAxisLabel[] YAxisLabels;

    private VariableSet ChosenSet;
    private Coroutine InitAnimCoroutine;
    private List<float> HeightOrder = new List<float>();
    private List<BarColor> BarColours = new List<BarColor>();
    private List<int> CorrectAnswer = new List<int>();
    private List<int> CorrectPresses = new List<int>();
    private bool CannotPress = true, Solved;
    private YAxisLabel yAxisLabel;

    private class VariableSet
    {
        public string Name { get; }
        public string[] Variables { get; }

        public VariableSet(string name, string[] variables)
        {
            Name = name;
            Variables = variables;
        }

        public VariableSet Copy()
        {
            return new VariableSet(Name, Variables);
        }

        public VariableSet Shuffle()
        {
            return new VariableSet(Name, Variables.ToArray().Shuffle().Take(4).ToArray());
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, string.Join(",", Variables));
        }
    }

    private enum BarColor : int
    {
        Red = 0,
        Yellow,
        Green,
        Blue
    }

    private Vector3 FindBarScale(float height, int type, float t)
    {
        if (t > 1f)
            t = 1f;
        if (t < 0)
            t = 0;
        if (type == 0)
            return new Vector3(1, (height * t) + ((height * t) - 2) * -0.06f, 1);
        if (type == 1)
            return new Vector3(1, (height * t) + ((height * t) - 2) * -0.025f, 1);
        return new Vector3(1, height * t, 1);
    }

    private Color FindBarColour(int ix, int type)
    {
        if (type == 0)
            return new[] { new Color(0.5f, 0, 0), new Color(0.5f, 0.5f, 0), new Color(0, 0.5f, 0), new Color(0, 0, 0.5f) }[ix];
        if (type == 1)
            return new[] { new Color(1, 0.5f, 0.5f), new Color(1, 1, 0.5f), new Color(0.5f, 1, 0.5f), new Color(0.5f, 0.5f, 1) }[ix];
        if (type == 2)
            return new Color(0.25f, 0.25f, 0.25f);
        return new Color(0.75f, 0.75f, 0.75f);
    }

    void SetupRuleseed()
    {
        MonoRandom rng = RuleSeed.GetRNG();
        if(rng.Seed == 1)
        {
            Debug.LogFormat("Ruleseed is 1.");
            VariableSets = AllVariableSets;
            ColorValues = Enumerable.Range(0, 4).ToDictionary(x => (BarColor)x, x => x);
            BarRules = new BarRule[] { BarRule.Leftmost, BarRule.Shortest, BarRule.FirstInOrder };
            YAxisLabels = new YAxisLabel[] { YAxisLabel.Popularity, YAxisLabel.Frequency };
        }
        else
        {
            VariableSets = AllVariableSets.Select(vs => new VariableSet(vs.Name,rng.ShuffleFisherYates(vs.Variables))).ToArray();
            Debug.LogFormat("<Bar Charts> Ruleseed {0}: {1}", rng.Seed, string.Join("\r\n", VariableSets.Select(v => v.ToString()).ToArray()));
            List<int> ColorValueToRandomize = rng.ShuffleFisherYates(Enumerable.Range(0, 4).ToList());
            ColorValues = Enumerable.Range(0, 4).ToDictionary(x => (BarColor)x, x => ColorValueToRandomize[x]);
            Debug.LogFormat("<Bar Charts> Colors : {0}", string.Join(",", ColorValues.Select(x => $"{x.Key}=>{x.Value}").ToArray()));
            BarRules = rng.ShuffleFisherYates(Enum.GetValues(typeof(BarRule)).Cast<BarRule>().ToList()).Take(3).ToArray();
            Debug.LogFormat("<Bar Charts> Rules : {0}", string.Join(",", BarRules.Select(b=>b.ToString()).ToArray()));
            YAxisLabels = rng.ShuffleFisherYates(Enum.GetValues(typeof(YAxisLabel)).Cast<YAxisLabel>().ToList()).Take(2).ToArray();
            Debug.LogFormat("<Bar Charts> Labels : {0}", string.Join(",", YAxisLabels.Select(l => l.ToString()).ToArray()));
        }
    }

    void Awake()
    {
        _moduleID = _moduleIdCounter++;
        for (int i = 0; i < Selectables.Length; i++)
        {
            int x = i;
            Highlights[x].gameObject.SetActive(false);
            Selectables[x].OnHighlight += delegate { if (!CannotPress && !CorrectPresses.Contains(x)) Highlights[x].gameObject.SetActive(true); };
            Selectables[x].OnHighlightEnded += delegate { if (!CannotPress) Highlights[x].gameObject.SetActive(false); };
            Selectables[x].OnInteract += delegate { if (!CannotPress && !CorrectPresses.Contains(x)) SelectBar(x); return false; };
            Selectables[x].OnInteractEnded += delegate { if (!CorrectPresses.Contains(x)) DeselectBar(x); else Selectables[x].OnHighlightEnded(); };
            Selectables[x].transform.localScale = Vector3.zero;
        }
        ModuleBG.material.color = Color.black;
        Module.OnActivate += delegate { ModuleBG.material.color = Color.white; CannotPress = false; InitAnimCoroutine = StartCoroutine(InitAnim()); };
    }

    // Use this for initialization
    void Start()
    {
        SetupRuleseed();
        Calculate();
    }

    void Calculate()
    {
        HeightOrder = Enumerable.Range(1, 4).Select(x => Rnd.Range(x - RandomTolerance, x + RandomTolerance)).ToList().Shuffle();
        var intHeightOrder = HeightOrder.Select(x => Mathf.RoundToInt(x)).ToList();
        yAxisLabel = YAxisLabels.PickRandom();
        UnitRend.text = yAxisLabel.ToString();
        BarColours = Enumerable.Range(0, 4).Select(i => (BarColor)i).ToList().Shuffle();
        ChosenSet = VariableSets.PickRandom().Copy();
        var shuffledSet = ChosenSet.Shuffle();
        for (int i = 0; i < BarTextRends.Length; i++)
            BarTextRends[i].text = shuffledSet.Variables[i];

        Debug.LogFormat("[Bar Charts #{0}] Bar sizes (1 = shortest, 4 = tallest): {1}.", _moduleID, intHeightOrder.Join(", "));
        Debug.LogFormat("[Bar Charts #{0}] Bar colours: {1}.", _moduleID, BarColours.Join(", "));
        Debug.LogFormat("[Bar Charts #{0}] Bar labels: {1} (category: {2}).", _moduleID, BarTextRends.Select(x => !Deabbreviator.ContainsKey(x.text) ? x.text : Deabbreviator[x.text]).Join(", "), ChosenSet.Name );

        var labelValues = new List<int>();
        for (int i = 0; i < BarTextRends.Length; i++)
            labelValues.Add(Array.IndexOf(ChosenSet.Variables, BarTextRends[i].text) + 1);

        var a = labelValues.Sum() % ChosenSet.Variables.Length;

        Debug.LogFormat("[Bar Charts #{0}] Label values [indexes into category's list]: {1}. A = {2}.", _moduleID, labelValues.Join(", "), a + 1);

        if (BarTextRends.Select(x => x.text).Contains(ChosenSet.Variables[a]))
        {
            CorrectAnswer.Add(Enumerable.Range(0, 4).Where(x => BarTextRends[x].text == ChosenSet.Variables[a]).First());
            Debug.LogFormat("[Bar Charts #{0}] Element {1} in the list, {2}, appears on the module, so its bar, Bar {3}, is first in the order.", _moduleID, a + 1, ChosenSet.Variables[a], CorrectAnswer.Last() + 1);
        }
        else
        {
            var aOld = a;
            a++;
            a %= 4;
            a++;
            Debug.LogFormat("[Bar Charts #{0}] Element {1} in the list, {2}, does not appear on the module, so A now equals {3}.", _moduleID, aOld + 1, ChosenSet.Variables[aOld], a);
            CorrectAnswer.Add(Enumerable.Range(0, 4).Where(x => intHeightOrder[x] == a).First());
            Debug.LogFormat("[Bar Charts #{0}] The {1}shortest bar is Bar {2}, so it is first in the order.", _moduleID, new[] { "", "second-", "third-", "fourth-" }[a - 1], CorrectAnswer.Last() + 1);
        }
        int[] bValues = GetBValues(intHeightOrder);
        var b = bValues.Sum() % 3;
        if (b < CorrectAnswer.First())
            CorrectAnswer.Add(b);
        else
            CorrectAnswer.Add(b + 1);
        Debug.LogFormat("[Bar Charts #{0}] The {1}, {2} and {3} have values {4}, {5} and {6} respectively. This means that Bar {7} is second in the order.",
            _moduleID, BarRules[0].ToLogString(), BarRules[1].ToLogString(), BarRules[2].ToLogString(),
            bValues[0], bValues[1], bValues[2], CorrectAnswer.Last() + 1);

        var competitors = Enumerable.Range(0, 4).Except(CorrectAnswer).ToList();
        Debug.LogFormat("[Bar Charts #{0}] The unit on the left is {1}.", _moduleID, yAxisLabel);
        IEnumerable<int> orderedCompetitors;
        switch (yAxisLabel)
        {
            case YAxisLabel.Popularity:
                orderedCompetitors = competitors.OrderBy(c => intHeightOrder[c]);
                break;
            case YAxisLabel.Frequency:
                orderedCompetitors = competitors.OrderByDescending(c => intHeightOrder[c]);
                break;
            case YAxisLabel.Responses:
                orderedCompetitors = competitors.OrderBy(c => c);
                break;
            case YAxisLabel.Occurances:
                orderedCompetitors = competitors.OrderByDescending(c => c);
                break;
            case YAxisLabel.Density:
                orderedCompetitors = competitors.OrderBy(c => ColorValues[BarColours[c]]);
                break;
            case YAxisLabel.Magnitude:
                orderedCompetitors = competitors.OrderByDescending(c => ColorValues[BarColours[c]]);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(yAxisLabel), yAxisLabel.ToString());
        }
        CorrectAnswer.AddRange(orderedCompetitors.ToList());
        Debug.LogFormat("[Bar Charts #{0}] Bar {1} {2} than Bar {3}, so it is third in the order, and Bar {2} is fourth.", _moduleID, orderedCompetitors.ToList()[0] + 1, yAxisLabel.GetComparativeWord(), orderedCompetitors.ToList()[1] + 1);
        Debug.LogFormat("[Bar Charts #{0}] This means that the final order is {1}.", _moduleID, CorrectAnswer.Select(x => x + 1).Join(", "));
    }

    int[] GetBValues(List<int> intHeightOrder)
    {
        int[] values = new int[3];
        for(int i = 0; i < 3; i++)
        {
            if ((int)BarRules[i] <= 3)
                values[i] = ColorValues[BarColours[(int)BarRules[i]]];
            else if (BarRules[i] != BarRule.FirstInOrder)
                values[i] = ColorValues[BarColours[intHeightOrder.IndexOf(BarRules[i] - BarRule.Shortest + 1)]];
            else
                values[i] = ColorValues[BarColours[CorrectAnswer.First()]];
        }
        return values;
    }

    void SelectBar(int pos)
    {
        Audio.PlaySoundAtTransform("press", Selectables[pos].transform);
        Selectables[pos].AddInteractionPunch();
        if (CorrectAnswer[CorrectPresses.Count()] == pos)
        {
            CorrectPresses.Add(pos);
            Debug.LogFormat("[Bar Charts #{0}] You pressed Bar {1}, which was correct.", _moduleID, pos + 1);
        }
        else
        {
            Module.HandleStrike();
            Debug.LogFormat("[Bar Charts #{0}] You pressed Bar {1}, which was incorrect — I expected Bar {2}. Strike!", _moduleID, pos + 1, CorrectAnswer[CorrectPresses.Count()] + 1);
        }
        if (CorrectPresses.Count() == 4)
            HandleSolve();
        if (!CannotPress)
            SetBarColour(pos, true);
    }

    void DeselectBar(int pos)
    {
        Audio.PlaySoundAtTransform("release", Selectables[pos].transform);
        SetBarColour(pos, false);
    }

    void SetBarColour(int pos, bool invert)
    {
        Bases[pos].color = FindBarColour((int)BarColours[pos], invert ? 1 : 0);
        Colourings[pos].color = FindBarColour((int)BarColours[pos], invert ? 0 : 1);
    }

    void HandleSolve()
    {
        Module.HandlePass();
        Audio.PlaySoundAtTransform("solve", Module.transform);
        Solved = true;
        CannotPress = true;
        Debug.LogFormat("[Bar Charts #{0}] Module solved!", _moduleID);
        foreach (var hl in Highlights)
            hl.gameObject.SetActive(false);
        if (InitAnimCoroutine != null)
            StopCoroutine(InitAnimCoroutine);
        StartCoroutine(HandleSolveAnim());
    }

    private IEnumerator InitAnim(float duration = 1f)
    {
        for (int i = 0; i < 4; i++)
        {
            Selectables[i].transform.localScale = new Vector3(2, 1, 2);
            Bases[i].color = FindBarColour((int)BarColours[i], 0);
            Colourings[i].color = FindBarColour((int)BarColours[i], 1);
            Highlights[i].transform.parent.localScale = FindBarScale(HeightOrder[i], 0, 0);
            Bases[i].transform.parent.localScale = FindBarScale(HeightOrder[i], 1, 0);
            Colourings[i].transform.parent.localScale = FindBarScale(HeightOrder[i], 2, 0);
        }
        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            var t = Easing.OutCubic(timer, 0, 1, duration);
            for (int i = 0; i < 4; i++)
            {
                Highlights[i].transform.parent.localScale = FindBarScale(HeightOrder[i], 0, t);
                Bases[i].transform.parent.localScale = FindBarScale(HeightOrder[i], 1, t);
                Colourings[i].transform.parent.localScale = FindBarScale(HeightOrder[i], 2, t);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            Highlights[i].transform.parent.localScale = FindBarScale(HeightOrder[i], 0, 1);
            Bases[i].transform.parent.localScale = FindBarScale(HeightOrder[i], 1, 1);
            Colourings[i].transform.parent.localScale = FindBarScale(HeightOrder[i], 2, 1);
        }
    }

    private IEnumerator HandleSolveAnim(float interval = 0.15f)
    {
        for (int i = 0; i < 4; i++)
        {
            Bases[i].color = FindBarColour(-1, 3);
            Colourings[i].color = FindBarColour(-1, 2);
        }
        StartCoroutine(FadeOut());
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(HandleBarLeave(i));
            float timer = 0;
            while (timer < interval)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
    }

    private IEnumerator HandleBarLeave(int pos, float duration = 0.65f, float cap = 4.5f)
    {
        var init = new List<float>();
        Highlights[pos].transform.parent.localScale = FindBarScale(HeightOrder[pos], 0, 1);
        init.Add(Highlights[pos].transform.parent.localScale.y);
        Bases[pos].transform.parent.localScale = FindBarScale(HeightOrder[pos], 1, 1);
        init.Add(Bases[pos].transform.parent.localScale.y);
        Colourings[pos].transform.parent.localScale = FindBarScale(HeightOrder[pos], 2, 1);
        init.Add(Colourings[pos].transform.parent.localScale.y);

        var caps = new List<float>() { FindBarScale(cap, 0, 1).y, FindBarScale(cap, 1, 1).y, FindBarScale(cap, 2, 1).y };
        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            Highlights[pos].transform.parent.localScale = new Vector3(1, Easing.OutSine(timer, init[0], caps[0], duration), 1);
            Bases[pos].transform.parent.localScale = new Vector3(1, Easing.OutSine(timer, init[1], caps[1], duration), 1);
            Colourings[pos].transform.parent.localScale = new Vector3(1, Easing.OutSine(timer, init[2], caps[2], duration), 1);
        }

        var ends = new List<float>() { FindBarScale(0, 0, 1).y, FindBarScale(0, 1, 1).y, FindBarScale(0, 2, 1).y };
        timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            Highlights[pos].transform.parent.localScale = new Vector3(1, Easing.InSine(timer, caps[0], ends[0], duration), 1);
            Bases[pos].transform.parent.localScale = new Vector3(1, Easing.InSine(timer, caps[1], ends[1], duration), 1);
            Colourings[pos].transform.parent.localScale = new Vector3(1, Easing.InSine(timer, caps[2], ends[2], duration), 1);
        }

        Highlights[pos].transform.parent.localScale = new Vector3(1, ends[0], 1);
        Bases[pos].transform.parent.localScale = new Vector3(1, ends[1], 2);
        Colourings[pos].transform.parent.localScale = new Vector3(1, ends[2], 1);
    }

    private IEnumerator FadeOut(float startDuration = 1.75f, float endDuration = 1f)
    {
        float timer = 0;
        while (timer < startDuration)
        {
            yield return null;
            timer += Time.deltaTime;
            UnitRend.color = Color.Lerp(Color.black, Color.clear, timer / startDuration);
            for (int i = 0; i < BarTextRends.Length; i++)
                BarTextRends[i].color = Color.Lerp(Color.black, Color.clear, timer / startDuration);
        }
        UnitRend.color = Color.clear;
        UnitRend.text = "";
        for (int i = 0; i < BarTextRends.Length; i++)
        {
            BarTextRends[i].color = Color.clear;
            BarTextRends[i].text = "";
        }
        timer = 0;
        while (timer < 0.25f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        for (int i = 0; i < 4; i++)
            Colourings[i].color = Color.clear;
        timer = 0;
        while (timer < endDuration)
        {
            yield return null;
            timer += Time.deltaTime;
            for (int i = 0; i < 4; i++)
                Bases[i].color = Color.Lerp(FindBarColour(-1, 3), Color.black, timer / endDuration);
            ModuleBG.material.color = Color.Lerp(Color.white, Color.black, timer / endDuration);
        }
        for (int i = 0; i < 4; i++)
            Bases[i].color = Color.clear;
        ModuleBG.material.color = Color.black;
    }

    private IEnumerator TPPress(int pos)
    {
        Selectables[pos].OnInteract();
        yield return null;
        Selectables[pos].OnInteractEnded();
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "Use '!{0} 1234' to press bars 1, 2, 3 and 4.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        foreach (var character in command)
        {
            if (!"1234".Contains(character))
            {
                yield return "sendtochaterror Invalid command.";
                yield break;
            }
            if (command.Count(x => x == character) > 1)
            {
                yield return "sendtochaterror You told me to press bar " + character + " more than once, which I can't do. Command cancelled.";
                yield break;
            }
            if (CorrectPresses.Contains(int.Parse(character.ToString()) - 1))
            {
                yield return "sendtochaterror Bar " + character + " has already been pressed. Command cancelled.";
                yield break;
            }
        }
        yield return null;
        foreach (var character in command)
        {
            StartCoroutine(TPPress(int.Parse(character.ToString()) - 1));
            float timer = 0;
            while (timer < 0.25f)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (CannotPress)
            yield return true;
        while (!Solved)
        {
            var ix = CorrectAnswer[CorrectPresses.Count()];
            yield return null;
            Selectables[ix].OnInteract();
            yield return null;
            Selectables[ix].OnInteractEnded();
        }
    }
}
