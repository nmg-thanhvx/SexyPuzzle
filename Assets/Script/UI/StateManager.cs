using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateManager : MonoBehaviour {
	public GameObject[] states;

	public bool addChildrenToStates = true;

	public string defaultState;

	GameObject _last = null;
	GameObject _current = null;
	bool _isStateChanging = false;
	Queue<Action> _changeStateQueue = new Queue<Action>();

	public event EventHandler<StateChangingEventArgs> stateChanging;

	public event EventHandler<StateChangedEventArgs> stateChanged;

	public GameObject last {
		get { return _last; }
	}

	public GameObject current {
		get { return _current; }
	}

	public string lastStateName {
		get { return _last ? _last.name : null; }
	}

	public string currentStateName {
		get { return _current ? _current.name : null; }
	}

	public GameObject GetState(string stateName) {
		return states.Where(s => s).FirstOrDefault(s => s.name == stateName);
	}

	public void ChangeState(string stateName, Dictionary<string, object> parameters = null, Action changed = null) {
		if (_isStateChanging) {
			// NOTE: OnEnterまたはOnExitの中でステートを変更しようとするとここを通る。
			// OnEnterとOnExitの実行順を保持するため、キューに詰めてステート変更後に呼び出す。
			_changeStateQueue.Enqueue(() => ChangeState(stateName, parameters, changed));
			return;
		}

		try {
			_isStateChanging = true;

			var nextState = GetState(stateName);
			if (nextState == null) {
				return;
			}

			// stateChangingイベント
			var changingArgs = new StateChangingEventArgs(currentStateName, nextState.name);
			OnStateChanging(changingArgs);
			if (changingArgs.cancel) {
				return;
			}

			// 前のステートのOnExit (currentはOnExitが呼ばれているステート)
			if (_current) {
				var lastState = (IState)_current.GetComponent(typeof(IState));
				if (lastState != null) {
					lastState.OnExit();
				}
			}

			// currentとlastの更新
			_last = _current;
			_current = null;

			// 次のステート以外を非アクティブにする
			foreach (var s in states) {
				if (s != nextState) {
					s.SetActive(false);
				}
			}

			// 次のステートのOnEnter (currentはOnEnterが呼ばれているステート)
			_current = nextState;
			_current.SetActive(true);
			var currentState = (IState)_current.GetComponent(typeof(IState));
			if (currentState != null) {
				currentState.OnEnter(parameters);
			}

			// stateChangedイベント
			if (changed != null) {
				changed();
			}
			OnStateChanged(new StateChangedEventArgs(lastStateName, currentStateName));

			return;
		} finally {
			_isStateChanging = false;

			while (_changeStateQueue.Count >= 1) {
				_changeStateQueue.Dequeue()();
			}
		}
	}

	protected virtual void OnStateChanging(StateChangingEventArgs e) {
		if (stateChanging != null) {
			stateChanging(this, e);
		}
	}

	protected virtual void OnStateChanged(StateChangedEventArgs e) {
		if (stateChanged != null) {
			stateChanged(this, e);
		}
	}

	void Awake() {
		if (addChildrenToStates) {
			states = states ?? new GameObject[0];
			states = states.Concat(transform.Cast<Transform>().Select(t => t.gameObject)).ToArray();
		}
	}

	void Start() {
		if (!_current && !string.IsNullOrEmpty(defaultState)) {
			ChangeState(defaultState);
		}
	}

	void OnDestroy() {
		if (_current) {
			var state = (IState)_current.GetComponent(typeof(IState));
			if (state != null) {
				state.OnExit();
			}
			_current = null;
		}
	}
}

// ステートとして使うゲームオブジェクトがコンポーネントとしてIStateを実装していると、OnEnterとOnExitを受け取れます。
// StateManagerでゲームオブジェクトのアクティブ状態を切り替えたいだけなら、IStateを実装している必要はありません。
public interface IState {
	void OnEnter(Dictionary<string, object> parameters);

	void OnExit();
}
public interface IStateListener
{
    void OnEnter();

    void OnExit();
}

public class StateChangingEventArgs : EventArgs {
	string _from;
	string _to;
	bool _cancel = false;

	public StateChangingEventArgs(string from, string to) {
		_from = from;
		_to = to;
	}

	public string from {
		get { return _from; }
	}

	public string to {
		get { return _to; }
	}

	public bool cancel {
		get { return _cancel; }
		set { _cancel = value; }
	}
}

public class StateChangedEventArgs : EventArgs {
	string _from;
	string _to;

	public StateChangedEventArgs(string from, string to) {
		_from = from;
		_to = to;
	}

	public string from {
		get { return _from; }
	}

	public string to {
		get { return _to; }
	}
}
