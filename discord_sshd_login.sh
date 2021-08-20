#!/bin/bash

WEBHOOK="CHANGE_ME!"

# Let's capture only open_session event (login).
case "$PAM_TYPE" in
    open_session)
        pam_discord_login $WEBHOOK $PAM_RHOST $HOSTNAME
        exit $?
        ;;
esac