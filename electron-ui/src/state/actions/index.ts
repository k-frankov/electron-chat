import { ActionType } from "../action-types";

interface UserLoggedInActoin {
    type: ActionType.USER_LOGGED_IN,
}

interface UserLoggedOutAction {
    type: ActionType.USER_LOGGED_OUT,
}

export type Action = UserLoggedInActoin | UserLoggedOutAction;