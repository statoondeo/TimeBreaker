using System;
using UnityEngine;

public class OutOfBoundPhysicController : MonoBehaviour
{
	public event Action Hitted;

	private void OnCollisionEnter2D(Collision2D collision) => Hitted?.Invoke();
}
