﻿"use strict";


const baseUrl = `${window.location.origin}/Notification/GetNotifCount`;

console.log('ID:', baseUrl);



fetch(baseUrl, {
    method: "GET"
})
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        console.log('Notification Count:', data);
        $('#notifCount').text(data);

    })
