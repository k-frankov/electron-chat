import { UserAuthAction } from "../actions/index"
import { ActionType } from "../action-types/index"
import { ChatUser } from "../models/chatUser";

const initialState = null;

const reducer = (state = initialState, action: UserAuthAction): ChatUser | null => {
    switch (action.type) {
        case ActionType.USER_LOGGED_IN:
            return action.payload;
        case ActionType.USER_LOGGED_OUT:
            return null;
        default: return state;
    }
}

export default reducer;