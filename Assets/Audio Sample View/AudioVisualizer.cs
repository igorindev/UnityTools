using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioVisualizer : MonoBehaviour
{
    [SerializeField] Mode mode;
    [Min(64)][SerializeField] int samples;
    [Min(1)][SerializeField] int UIBars;
    [SerializeField] GameObject[] bars;

    [Space(5)]
    [Header("Instance")]
    public GameObject barPrefab;
    [Tooltip("Horizontal Group que recebe as barras")]
    public Transform horizontalGroup;

    public static float[] _samples;
    public static float[] freqBand;
    public static float[] bandBuffer;
    public static float[] audioBand;
    public static float[] audioBandBuffer;

    private AudioSource _audioSource;
    private float[] bufferDecrease;
    private float[] freqBandHighest;

    public enum Mode
    {
        InstantiateBars,
        UseReference
    }

    private void Awake()
    {
        _samples = new float[samples];
        freqBand = new float[UIBars];
        bandBuffer = new float[UIBars];
        bufferDecrease = new float[UIBars];
        freqBandHighest = new float[UIBars];
        audioBand = new float[UIBars];
        audioBandBuffer = new float[UIBars];
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        //Spawn bars
        bars = new GameObject[UIBars];

        if (mode == Mode.InstantiateBars)
        {
            for (int i = 0; i < UIBars; i++)
            {
                GameObject obj = Instantiate(barPrefab, horizontalGroup);
                obj.name = "SampleCube " + i;
                bars[i] = obj;
                bars[i].GetComponent<ParamCube>().band = i;
            }
        }
    }

    private void Update()
    {
        for (int i = 1; i < _samples.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, _samples[i] + 10, 0), new Vector3(i, _samples[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(_samples[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(_samples[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), _samples[i - 1] - 10, 1), new Vector3(Mathf.Log(i), _samples[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(_samples[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(_samples[i]), 3), Color.blue);
        }

        GetSpectrumAudioSource();
        TestMakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
    }

    private void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    private void MakeFrequencyBands()
    {
        int count = 0;

        for (int i = 0; i < UIBars; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= sampleCount;

            freqBand[i] = average * 10;
        }
    }

    private void TestMakeFrequencyBands()
    {
        int band = 0;

        for (int i = 0; i < freqBand.Length; i++)
        {
            float average = 0;
            // As you increment on Frequency bands to set, get number of samples looking to get average of next based on for loop progress percentage
            int sampleCount = (int)Mathf.Lerp(2f, _samples.Length - 1, i / ((float)freqBand.Length - 1));

            // always start the j index at the current value of band here!  if you always start from 0, band++ will increment out of _samples bounds
            for (int j = band; j < sampleCount; j++)
            {
                average += _samples[band] * (band + 1);
                band++;
            }

            average /= band;

            freqBand[i] = average * 10;
        }
    }

    private void BandBuffer()
    {
        for (int i = 0; i < UIBars; ++i)
        {
            if (freqBand[i] > bandBuffer[i])
            {
                bandBuffer[i] = freqBand[i];
                bufferDecrease[i] = 0.005f;
            }
            if (freqBand[i] < bandBuffer[i])
            {
                bandBuffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f;
            }
        }
    }

    private void CreateAudioBands()
    {
        for (int i = 0; i < UIBars; i++)
        {
            if (freqBand[i] > freqBandHighest[i])
            {
                freqBandHighest[i] = freqBand[i];
            }
            audioBand[i] = (freqBand[i] / freqBandHighest[i]);
            audioBandBuffer[i] = Mathf.Clamp01((bandBuffer[i] / freqBandHighest[i]));
        }
    }
}
