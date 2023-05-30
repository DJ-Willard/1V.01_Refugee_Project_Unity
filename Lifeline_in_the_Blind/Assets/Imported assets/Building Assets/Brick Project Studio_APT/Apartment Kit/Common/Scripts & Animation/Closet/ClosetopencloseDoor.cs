using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles

{
	public class ClosetopencloseDoor : MonoBehaviour
	{

		public Animator Closetopenandclose;
		public bool open;
		public Transform Player;

		void Start()
		{
			open = false;
		}

		// PW addition
		public void sendMessageTest()
		{
			Debug.Log("Different script sucessfully called sendMessageTest().");
		}

		// PW: This is the built-in functionality but it doesn't open the door every time.
		public void UseDoor()
		{

			if (open == false)
			{
				Debug.Log("opening...");
				StartCoroutine(opening());
			}
			else
			{
				Debug.Log("closing...");
				StartCoroutine(closing());
			}

		}

		IEnumerator opening()
		{
			print("you are opening the door");
			Closetopenandclose.Play("ClosetOpening");
			open = true;
			yield return new WaitForSeconds(.5f);
		}

		IEnumerator closing()
		{
			print("you are closing the door");
			Closetopenandclose.Play("ClosetClosing");
			open = false;
			yield return new WaitForSeconds(.5f);
		}


	}
}