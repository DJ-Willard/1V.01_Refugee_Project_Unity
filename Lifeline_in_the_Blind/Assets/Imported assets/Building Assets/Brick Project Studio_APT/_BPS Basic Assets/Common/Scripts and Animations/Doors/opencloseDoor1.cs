using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles

{
	public class opencloseDoor1 : MonoBehaviour
	{

		public Animator openandclose;
		public bool open;
		public Transform Player;

		void Start()
		{
			open = false;
		}

		// PW: This is the built-in functionality but it doesn't open the door every time.
		void OnMouseOver()
		{
			{
				if (Player)
				{
					float dist = Vector3.Distance(Player.position, transform.position);
					Debug.Log("Distance to door = " + dist);
					if (dist < 15)
					{
						if (open == false)
						{
							//if (Input.GetMouseButtonDown(0))
							if (Input.GetKeyDown("e"))
							{
								Debug.Log("opening...");
								StartCoroutine(opening());
							}
						}
						else
						{
							if (open == true)
							{
								//if (Input.GetMouseButtonDown(0))
								if (Input.GetKeyDown("e"))
								{
									Debug.Log("closing...");
									StartCoroutine(closing());
								}
							}

						}

					}
				}

			}

		}

		IEnumerator opening()
		{
			print("you are opening the door");
			openandclose.Play("Opening");
			open = true;
			yield return new WaitForSeconds(.5f);
		}

		IEnumerator closing()
		{
			print("you are closing the door");
			openandclose.Play("Closing");
			open = false;
			yield return new WaitForSeconds(.5f);
		}


	}
}