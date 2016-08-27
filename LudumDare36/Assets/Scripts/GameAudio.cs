using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameAudio : MonoBehaviour
{
	private List<AudioSource> listSfx;
	private int indexSFx = 0;

	private List<AudioSource> listSfxLoop;
	private int indexSFxLoop = 0;

	public AudioClip trackA;
	public AudioClip trackAB;
	public AudioClip trackB;
	
	private AudioClip nextTrackLoop = null;
	private AudioClip nextTrackTransition = null;
	private int nextTrackTransitionDelay = 0;

	private AudioSource currentTrack = null;
	private int currentTrackTransitionDelay = 0;
	
	void Awake ()
	{
		//SFx
		listSfx = new List<AudioSource>();
		for (int i = 0; i < 32; ++i)
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
		const float beat1 = (1.0f * 60.0f) / 124.0f;
		const float beat4 = (4.0f * 60.0f) / 124.0f;
		const float beat8 = (8.0f * 60.0f) / 124.0f;

		print(beat1);
		print(beat4);
		print(beat8);

		InvokeRepeating("UpdateTrack", 0.0f, beat4);
	}

	void OnGUI()
	{
        if (GUI.Button(new Rect(10, 10, 150, 50), "Play A"))
		{
            nextTrackLoop = trackA;
            nextTrackTransition = null;
			nextTrackTransitionDelay = 0;
		}
        else if (GUI.Button(new Rect(10, 70, 150, 50), "Play B"))
		{
            nextTrackLoop = trackB;
            nextTrackTransition = trackAB;
			nextTrackTransitionDelay = 1;
		}
    }

	void UpdateTrack()
	{
		if (currentTrackTransitionDelay > 0)
		{
			--currentTrackTransitionDelay;
			return;
		}

		if (nextTrackTransition)
		{
			if (currentTrack)
			{
				StopSFxLoop(currentTrack);
			}
			
	        currentTrack = PlaySFxLoop(nextTrackTransition);

			nextTrackTransition = null;
			currentTrackTransitionDelay = nextTrackTransitionDelay;
			nextTrackTransitionDelay = 0;
		}
		else if (nextTrackLoop)
		{
			if (currentTrack)
			{
				StopSFxLoop(currentTrack);
			}

	        currentTrack = PlaySFxLoop(nextTrackLoop);

			nextTrackLoop = null;
			currentTrackTransitionDelay = 0;
		}
    }

	public void PlaySFx(AudioClip _pClip)
	{
		if (_pClip)
		{
			AudioSource pSource = listSfx[indexSFx];
			++indexSFx;
			if (indexSFx >= listSfx.Count)
				indexSFx = 0;

			pSource.clip = _pClip;
			pSource.loop = false;
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
}
