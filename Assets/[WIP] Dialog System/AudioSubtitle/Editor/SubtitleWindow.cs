using UnityEditor;
using UnityEngine;

class SubtitleWindow : EditorWindow
{
	SOSubtitle _subtitle;
	SOSubtitle _lastSubtitle;

	Texture2D _lines;		// buffer for the lines
	Texture2D _waveform;	// buffer for the waveform
	Texture2D _display;		// buffer for what actually gets displayed (lines + waveform)

	float _time;
	float _timeCurrent;
	int _sampleCount;
	Color32[] _clear;

	
	GameObject _tempGameObject;
	AudioSource _tempAudioSource;
	
	GameObject GetTempGameObject
	{
		get
		{
			if(_tempGameObject == null) _tempGameObject = new GameObject();
			_tempGameObject.hideFlags = HideFlags.DontSave;
			_tempGameObject.name = "Subtitle Audio Source (temp)";
			return _tempGameObject;
		}
	}

	AudioSource GetTempAudioSource
	{
		get
		{
			if(_tempAudioSource == null) _tempAudioSource = GetTempGameObject.AddComponent<AudioSource>();
			return _tempAudioSource;
		}
	}
	
	[MenuItem ("Tools/Localization/Subtitler...")]
    public static void  ShowWindow () {
		SubtitleWindow window = GetWindow<SubtitleWindow>();
	    window.titleContent = new GUIContent("Subtitler");
		window.Show();

    }

    void OnDisable()
    {
	    if(_tempGameObject != null) DestroyImmediate(_tempGameObject);
	    _tempAudioSource = null;
	    _tempGameObject = null;
    }

    void OnGUI()
    {
	    if (Application.isPlaying)
	    {
		    GUILayout.Label("Edit mode only, my friend.", EditorStyles.boldLabel);
		    return;
	    }
	    
	    GUILayout.Label("Subtitle SO", EditorStyles.boldLabel);
	    EditorGUILayout.BeginHorizontal();
	    _subtitle = (SOSubtitle)EditorGUILayout.ObjectField(_subtitle, typeof(SOSubtitle), false);
	    if (GUILayout.Button("Refresh")) _lastSubtitle = null;
	    EditorGUILayout.EndHorizontal();

	    // Refresh on change
	    if (_subtitle != _lastSubtitle)
	    {
		    _waveform = null;
		    _lines = null;
		    _time = 0;
		    _timeCurrent = 0;
	    }
	    _lastSubtitle = _subtitle;
	    
	    
	    if(_subtitle == null)
	    {
		    GUILayout.Label("no subtitle selected", EditorStyles.boldLabel);
	    }
	    else
	    {
		    if (_subtitle.clip == null)
		    {
			    GUILayout.Label("subtitle is missing clip...", EditorStyles.boldLabel);
		    }
		    else
		    {
			    // redraw if the window changed size
			    if (_waveform == null || _waveform.width != (int) position.width)
			    {
				    _waveform = PaintWaveformSpectrum(_subtitle.clip, (int) position.width, 100, Color.black);
				    
				    if(_waveform != null && _display!=null &&
				       _waveform.width == _display.width  && _waveform.height == _display.height)
						Graphics.CopyTexture(_waveform, _display);
				    else
				    {
					    _lastSubtitle = null;
				    }
				    _sampleCount = _subtitle.clip.samples; //AudioUtility.GetSampleCount(_subtitle.clip);
				    if (_subtitle.length != _subtitle.clip.length)
				    {
					    _subtitle.length = _subtitle.clip.length;
					    EditorUtility.SetDirty(_subtitle);
				    }
				    PaintLinesOnwaveform();
			    }
			    
			    // show clip name
			    GUILayout.Label("Clip name: " + _subtitle.clip.name + "  (" + _subtitle.clip.length.ToString("0.00") + "s)", EditorStyles.boldLabel);
			    if (Event.current.isMouse && Event.current.button == 0 &&
			        Event.current.mousePosition.y > 80 && Event.current.mousePosition.y < 180)
			    {
				    _time = Event.current.mousePosition.x / (float) position.width;
				    PlayAudioclip(_subtitle.clip, _time);
			    }

			    // calculate moving line
			    if (GetTempAudioSource.isPlaying)
			    {
				    _timeCurrent = GetTempAudioSource.time / _subtitle.clip.length;
				    PaintLinesOnwaveform();
			    }

			    // draw the preview
			    PaintLinesOnwaveform();
			    EditorGUI.DrawPreviewTexture(new Rect(0, 80, (int) position.width, 100), _display);
			    EditorGUI.DrawPreviewTexture(new Rect(0, 180, (int) position.width, 20), _lines);
		    }

		    EditorGUILayout.Space();
		    for (int i = 0; i < 7; i++)
		    {	
			    GUILayout.Label("");
		    }
		    
		    GUILayout.Label("FMOD event: " + _subtitle.soundEvent);
		    GUILayout.Label("Localization term: " + _subtitle.localizationTerm);
		    GUILayout.Label("Lines", EditorStyles.boldLabel);
		    
		    // reset button
		    if (GUILayout.Button("RESET ALL"))
		    {
			    for (int i = 0; i < _subtitle.timestamps.Count; i++)
			    {
				    _subtitle.timestamps[i] = 0;
			    }
			    EditorUtility.SetDirty(_subtitle);
		    }
		    
		    
		    // Rows of subtitles
		    for (int i = 0; i < _subtitle.lines.Count; i++)
		    {
			    EditorGUILayout.BeginHorizontal();

			    if (GUILayout.Button("Set"))
			    {
				    _subtitle.timestamps[i] = _time * _subtitle.clip.length;
				    EditorUtility.SetDirty(_subtitle);
			    }
			    if (GUILayout.Button("Play"))
			    {
				    var normalizedTime = _subtitle.timestamps[i] / _subtitle.clip.length;
				    _time = normalizedTime;
				    PlayAudioclip(_subtitle.clip, normalizedTime);
			    }
			    
			    var length = ((i + 1 < _subtitle.lines.Count ? _subtitle.timestamps[i + 1] : _subtitle.length) -
			                  _subtitle.timestamps[i]);
			    
			    string warning = (length < 0.75f ? "(< 0.75 seconds !!!!!!!!!!!!!!!!!!!!!!!!!!!!" : "");
			    EditorGUILayout.LabelField(i + ". "  + _subtitle.lines[i] + warning,
				    GUILayout.Width(position.width * 0.65f));
			    
			    if(_subtitle.timestamps.Count<=i) _subtitle.timestamps.Add(0);
			    
			    EditorGUILayout.LabelField("[Chars: " + _subtitle.lines[i].Length.ToString() + "]", GUILayout.Width(position.width * 0.08f));
			    
			    
			    var origFontStyle = EditorStyles.label.fontStyle;
			    var origColor = EditorStyles.label.normal.textColor;

			    if (_subtitle.timestamps[i] == 0)
			    {
				    EditorStyles.label.fontStyle = FontStyle.Bold;
				    EditorStyles.label.normal.textColor = Color.red;
			    }

			    EditorGUILayout.LabelField("[S: " + _subtitle.timestamps[i].ToString("0.00") + "s]",
				    GUILayout.Width(position.width * 0.08f));
			    EditorStyles.label.fontStyle = origFontStyle;
			    EditorStyles.label.normal.textColor = origColor;
			    EditorGUILayout.LabelField("[Len: " + (length).ToString("0.00") + "s]" , 
				    GUILayout.Width(position.width * 0.08f));
			    
			    EditorGUILayout.EndHorizontal();    
		    }
	    }
    }

    void PlayAudioclip(AudioClip clip, float normalizedTime)
    {
	    var time = normalizedTime * clip.length;
	    _tempAudioSource.Stop();
	    _tempAudioSource.clip = clip;
	    _tempAudioSource.time = time;
	    _tempAudioSource.Play();
    }

    void Update()
    {
	    if(_subtitle != null && _subtitle.clip != null && GetTempAudioSource.isPlaying)
			Repaint();
    }

    // Used from inside a custom inspector on SubtitleSO's
    public static void ForceSubtitle(SOSubtitle subtitle)
    {
	    var window = (SubtitleWindow)GetWindow(typeof(SubtitleWindow));
	    window._subtitle = subtitle;
		window.Repaint();
    }

    // Waveform painting from audio-source
    public Texture2D PaintWaveformSpectrum(AudioClip audio, int width, int height, Color color) {
	    Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
	    float[] samples = new float[audio.samples * audio.channels];
	    float[] waveform = new float[width];
	    audio.GetData(samples, 0);

	    float packSize = ( samples.Length / (float)width ) + 1;

	    int s = 0;
	    for (float i = 0; i < samples.Length-1; i += packSize) {
		    Debug.Log(i + " " + samples.Length + " " +s + " " + waveform.Length);
		    waveform[s] = Mathf.Abs(samples[Mathf.RoundToInt(i)]);
		    s++;
	    }
 
	    // background
	    for (int x = 0; x < width; x++) {
		    for (int y = 0; y < height; y++) {
			    tex.SetPixel(x, y, Color.grey);
		    }
	    }
 
		// waveform
	    for (int x = 0; x < waveform.Length; x++) {
		    for (int y = 0; y <= waveform[x] * ((float)height * .75f); y++) {
			    tex.SetPixel(x, ( height / 2 ) + y, color);
			    tex.SetPixel(x, ( height / 2 ) - y, color);
		    }
	    }
	    
	    tex.Apply();
 
	    return tex;
    }

    // progress lines painmting
    public void PaintLinesOnwaveform()
    {
	    if(_display==null || _display.width != _waveform.width) _display = new Texture2D(_waveform.width, _waveform.height, TextureFormat.RGBA32, false);
	    if (_lines==null || _lines.width != _waveform.width) _lines = new Texture2D(_waveform.width, _waveform.height, TextureFormat.RGBA32, false);


	    if (_clear == null || _clear.Length != _lines.GetPixels32().Length)
	    {
		    _clear = new Color32[_lines.GetPixels32().Length];
		    for (int i = 0; i < _clear.Length; i++)
		    {
			    _clear[i] = new Color32(0,0,0,0); 
		    }
	    }
	    _lines.SetPixels32(_clear);
	    
	    // bars
	    for (int y = 0; y <= 100; y++) {
		    _lines.SetPixel((int) (_time * _lines.width), ( _lines.height / 2 ) + y, Color.white);
		    _lines.SetPixel((int)(_time * _lines.width), ( _lines.height / 2 ) - y, Color.blue);
	    }
	    
	    for (int y = 0; y <= 100; y++) {
		    _lines.SetPixel((int) (_timeCurrent * _lines.width), ( _lines.height / 2 ) + y, Color.white);
		    _lines.SetPixel((int)(_timeCurrent * _lines.width), ( _lines.height / 2 ) - y, Color.blue);
	    }
	    
	    _lines.Apply();
	    
	    // TODO: For some reason this stopped working at some point and I never fixed it...
	    // The line used to be drawn on top of the main waveform texture,
	    // but there was something broken and I don't remember really...
	    
	    //Graphics.CopyTexture(_waveform, _display);
	    //Graphics.CopyTexture(_lines, _display);
	    //_display.Apply();
    }
  
}