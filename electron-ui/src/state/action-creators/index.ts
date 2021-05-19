import { ActionType } from "../action-types";
import { Dispatch } from "redux";
import { Action } from "../actions";

export const logIn = () => {
    return (dispatch: Dispatch<Action>) => {
        dispatch({
            type: ActionType.USER_LOGGED_IN
        })
    }
}

export const logOut = () => {
    return (dispatch: Dispatch<Action>) => {
        dispatch({
            type: ActionType.USER_LOGGED_OUT
        })
    }
}