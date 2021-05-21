import { Action } from "../actions/index"
import { ActionType } from "../action-types/index"

const initialState = false;

const reducer = (state: boolean = initialState, action: Action): boolean => {
    switch (action.type) {
        case ActionType.USER_LOGGED_IN:
        case ActionType.USER_LOGGED_OUT:
            return !state;
        default: return state;
    }
}

export default reducer;