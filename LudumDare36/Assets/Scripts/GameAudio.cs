﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum ESFx
{
	Ring3310,
	Whoosh,
	Explode,
}

public enum EAudioLayer
{
	Wind,
	AwakenA,
	AwakenB,
	FightA,
	BossA,
	Final,
}

public enum EAudioLayerState
{
	Undefined,

	Idle,
	Play,
	FadeIn,
	FadeOut,
}

public class AudioLayer
{
	public EAudioLayer layer;
	public AudioClip clip;
	public AudioSource source = null;
	public float maxVolume = 1.0f;
	public bool loop = true;

	public EAudioLayerState command = EAudioLayerState.Idle;
	public EAudioLayerState state = EAudioLayerState.Idle;
}


public class GameAudio : MonoBehaviour
{
	//--------------------------------------------------------------
	
	public bool debugAudio = false;

	public AudioClip ambientWind;
	
	public AudioClip sfx3310;
	public AudioClip sfxWhoosh;
	public AudioClip sfxExplode;

	public AudioClip trackAwakenA;
	public AudioClip trackAwakenB;
	public AudioClip trackFightA;
	public AudioClip trackBossA;
	public AudioClip trackFinal;

	private List<AudioLayer> layers = new List<AudioLayer>();

	private float fadeInTime = 1.5f;
	private float fadeOutTime = 1.5f;

	public float beatDuration = 1.0f;
	private float timeSinceBeat = 0.0f;

	//--------------------------------------------------------------

	private List<AudioSource> listSfx;
	private int indexSFx = 0;

	private List<AudioSource> listSfxLoop;
	private int indexSFxLoop = 0;

	private List<AudioSource> listSfxConst;
	
	//--------------------------------------------------------------
	
	void Awake ()
	{
		layers.Add(new AudioLayer() { layer = EAudioLayer.Wind, clip = ambientWind } );
		layers.Add(new AudioLayer() { layer = EAudioLayer.AwakenA, clip = trackAwakenA } );
		layers.Add(new AudioLayer() { layer = EAudioLayer.AwakenB, clip = trackAwakenB } );
		layers.Add(new AudioLayer() { layer = EAudioLayer.FightA, clip = trackFightA } );
		layers.Add(new AudioLayer() { layer = EAudioLayer.BossA, clip = trackBossA } );
		layers.Add(new AudioLayer() { layer = EAudioLayer.Final, clip = trackFinal, loop = false } );

		listSfxConst = new List<AudioSource>();
		for (int i = 0; i < 16; ++i)
		{
			GameObject pObjectSFx = new GameObject("SFx_Const_"+i);
			AudioSource pAudioSFx = pObjectSFx.AddComponent<AudioSource>();
			pObjectSFx.transform.parent = gameObject.transform;

			listSfxConst.Add(pAudioSFx);

			if (i < layers.Count)
			{
				layers[i].source = pAudioSFx;
				pAudioSFx.clip = layers[i].clip;
				pAudioSFx.loop = layers[i].loop;
				pAudioSFx.playOnAwake = false;
				pAudioSFx.volume = 0.0f;
			}
		}

		//SFx
		listSfx = new List<AudioSource>();
		for (int i = 0; i < 16; ++i)
		{
			GameObject pObjectSFx = new GameObject("SFx_"+i);
			AudioSource pAudioSFx = pObjectSFx.AddComponent<AudioSource>();
			pObjectSFx.transform.parent = gameObject.transform;

			listSfx.Add(pAudioSFx);
		}

		listSfxLoop = new List<AudioSource>();
		for (int i = 0; i < 16; ++i)
		{
			GameObject pObjectSFx = new GameObject("SFx_Loop_"+i);
			AudioSource pAudioSFx = pObjectSFx.AddComponent<AudioSource>();
			pObjectSFx.transform.parent = gameObject.transform;
			
			listSfxLoop.Add(pAudioSFx);
		}
	}

	void Start()
	{
		// 124 BPM
		// 1 beat = 1 minute / 124 bpm = 
		//const float beat = (1.0f * 60.0f) / 124.0f;
		//const float beat = (4.0f * 60.0f) / 124.0f;
		const float beat = (8.0f * 60.0f) / 124.0f;
		beatDuration = beat;
		InvokeRepeating("UpdateTrack", 0.0f, beat);
	}

	public float GetTimeUntilBeat()
	{
		return Mathf.Max(0.0f, beatDuration - timeSinceBeat);
	}

	void UpdateTrack()
	{
		timeSinceBeat = 0.0f;

		foreach (AudioLayer layer in layers)
		{
			if (layer.command == EAudioLayerState.Undefined)
				continue;
			
			if (layer.command == layer.state)
			{
				layer.command = EAudioLayerState.Undefined;
				continue;
			}

			layer.state = layer.command;
			layer.command = EAudioLayerState.Undefined;
			
			if (layer.state == EAudioLayerState.Idle)
			{
				layer.source.volume = 0.0f;
				layer.source.Stop();
			}
			else if (layer.state == EAudioLayerState.Play)
			{
				layer.source.volume = layer.maxVolume;
				layer.source.Play();
			}
			else if (layer.state == EAudioLayerState.FadeIn)
			{
				layer.source.Play();
			}
		}
    }
	
	void Update()
	{
		timeSinceBeat += Time.deltaTime;

		foreach (AudioLayer layer in layers)
		{
			if (layer.state == EAudioLayerState.FadeIn)
			{
				if (layer.source.volume < layer.maxVolume)
				{
					layer.source.volume += Time.deltaTime * layer.maxVolume / fadeInTime;
					layer.source.volume = Mathf.Min(layer.source.volume, layer.maxVolume);
				}
				else
				{
					layer.state = EAudioLayerState.Play;
				}
			}
			else if (layer.state == EAudioLayerState.FadeOut)
			{
				if (layer.source.volume > 0.0f)
				{
					layer.source.volume -= Time.deltaTime * layer.maxVolume / fadeOutTime;
					layer.source.volume = Mathf.Max(layer.source.volume, 0.0f);
				}
				else
				{
					layer.state = EAudioLayerState.Idle;
					layer.source.Stop();
				}
			}
		}
	}

	public void PlayLayer(EAudioLayer index, bool fade)
	{
		AudioLayer layer = layers.Find(item => item.layer == index);
		if (layer != null && layer.source != null)
		{
			layer.command = EAudioLayerState.Undefined;
			
			layer.source.Play();

			if (fade)
			{
				layer.state = EAudioLayerState.FadeIn;
			}
			else
			{
				layer.state = EAudioLayerState.Play;
				layer.source.volume = layer.maxVolume;
			}
		}
	}

	public void StopLayer(EAudioLayer index, bool fade)
	{
		AudioLayer layer = layers.Find(item => item.layer == index);
		if (layer != null && layer.source != null)
		{
			layer.command = EAudioLayerState.Undefined;
			
			if (fade)
			{
				layer.state = EAudioLayerState.FadeOut;
			}
			else
			{
				layer.state = EAudioLayerState.Idle;
				layer.source.volume = 0.0f;
				layer.source.Stop();
			}
		}
	}

	public void PlayLayerOnBeatSync(EAudioLayer index, bool fade)
	{
		AudioLayer layer = layers.Find(item => item.layer == index);
		if (layer != null && layer.source != null)
		{
			layer.command = (fade) ? EAudioLayerState.FadeIn : EAudioLayerState.Play;
		}
	}
	
	public void StopLayerOnBeatSync(EAudioLayer index, bool fade)
	{
		AudioLayer layer = layers.Find(item => item.layer == index);
		if (layer != null && layer.source != null)
		{
			layer.command = (fade) ? EAudioLayerState.FadeOut : EAudioLayerState.Idle;
		}
	}

	public void PlaySFx(ESFx sfx)
	{
		if (sfx == ESFx.Ring3310)
			PlaySFx(sfx3310, 0.5f);
		else if (sfx == ESFx.Whoosh)
			PlaySFx(sfxWhoosh, 0.3f);
		else if (sfx == ESFx.Explode)
			PlaySFx(sfxExplode, 0.2f);
	}

	public AudioSource PlaySFxLoop(ESFx sfx)
	{
		if (sfx == ESFx.Ring3310)
			return PlaySFxLoop(sfx3310);
		else if (sfx == ESFx.Whoosh)
			return PlaySFxLoop(sfxWhoosh);
		return null;
	}

	public void PlaySFx(AudioClip _pClip, float volume)
	{
		if (_pClip)
		{
			AudioSource pSource = listSfx[indexSFx];
			++indexSFx;
			if (indexSFx >= listSfx.Count)
				indexSFx = 0;

			pSource.clip = _pClip;
			pSource.loop = false;
			pSource.volume = volume;
			pSource.Play();
		}
	}

	public AudioSource PlaySFxLoop(AudioClip clip)
	{
		if (clip)
		{
			AudioSource pSource = listSfxLoop[indexSFxLoop];
			++indexSFxLoop;
			if (indexSFxLoop >= listSfxLoop.Count)
				indexSFxLoop = 0;
			
			pSource.clip = clip;
			pSource.loop = true;
			pSource.Play();
			return pSource;
		}
		return null;
	}

	public void StopSFxLoop(AudioSource _pSource)
	{
		if (_pSource)
		{
			_pSource.Stop();
		}
	}
	
	public void StopAllSFxLoops()
	{
		foreach (AudioSource pSource in listSfxLoop)
		{
			pSource.Stop();
		}
	}
	
	//--------------------------------------------------------------
	
	void OnGUI()
	{
		if (!debugAudio)
			return;

		int x = 0;
		int y = 0;
		int w = 150;
		int h = 40;
		
        if (GUI.Button(new Rect(x, y, w, h), "Play Wind (fade)"))
		{
			PlayLayer(EAudioLayer.Wind, true);
		}
		
		x += w+5;
		y += 0;
        if (GUI.Button(new Rect(x, y, w, h), "Stop Wind (fade)"))
		{
			StopLayer(EAudioLayer.Wind, true);
		}

		x  = 0;
		y += h+5;
        if (GUI.Button(new Rect(x, y, w, h), "Play Fight (no fade)"))
		{
			PlayLayerOnBeatSync(EAudioLayer.FightA, false);
		}
		
		x += w+5;
		y += 0;
        if (GUI.Button(new Rect(x, y, w, h), "Stop Fight (no fade)"))
		{
			StopLayerOnBeatSync(EAudioLayer.FightA, false);
		}
		
		x += w+5;
		y += 0;
        if (GUI.Button(new Rect(x, y, w, h), "Play Fight (fade)"))
		{
			PlayLayerOnBeatSync(EAudioLayer.FightA, true);
		}
		
		x += w+5;
		y += 0;
        if (GUI.Button(new Rect(x, y, w, h), "Stop Fight (fade)"))
		{
			StopLayerOnBeatSync(EAudioLayer.FightA, true);
		}

		x  = 0;
		y += h+5;
        if (GUI.Button(new Rect(x, y, w, h), "Play Boss (no fade)"))
		{
			PlayLayerOnBeatSync(EAudioLayer.BossA, false);
		}
		
		x += w+5;
		y += 0;
        if (GUI.Button(new Rect(x, y, w, h), "Stop Boss (no fade)"))
		{
			StopLayerOnBeatSync(EAudioLayer.BossA, false);
		}
		
		x += w+5;
		y += 0;
        if (GUI.Button(new Rect(x, y, w, h), "Play Boss (fade)"))
		{
			PlayLayerOnBeatSync(EAudioLayer.BossA, true);
		}
		
		x += w+5;
		y += 0;
        if (GUI.Button(new Rect(x, y, w, h), "Stop Boss (fade)"))
		{
			StopLayerOnBeatSync(EAudioLayer.BossA, true);
		}
		
		x  = 0;
		y += h+5;
        if (GUI.Button(new Rect(x, y, w, h), "Play Awaken A"))
		{
			PlayLayerOnBeatSync(EAudioLayer.AwakenA, false);
		}
		
		x += w+5;
		y += 0;
        if (GUI.Button(new Rect(x, y, w, h), "Stop Awaken A"))
		{
			StopLayerOnBeatSync(EAudioLayer.AwakenA, false);
		}
		
		x  = 0;
		y += h+5;
        if (GUI.Button(new Rect(x, y, w, h), "Play Awaken B"))
		{
			PlayLayerOnBeatSync(EAudioLayer.AwakenB, true);
		}
		
		x += w+5;
		y += 0;
        if (GUI.Button(new Rect(x, y, w, h), "Stop Awaken B"))
		{
			StopLayerOnBeatSync(EAudioLayer.AwakenB, false);
		}
    }
	
	//--------------------------------------------------------------
	
	//Singleton variable
    private static GameAudio s_instance = null;

    //GameManager singleton declaration
    public static GameAudio instance
    {
        get
        {
            //Get instance in current scene
            if (s_instance == null)
            {
                s_instance = FindObjectOfType(typeof(GameAudio)) as GameAudio;
            }

            return s_instance;
        }
    }
}
