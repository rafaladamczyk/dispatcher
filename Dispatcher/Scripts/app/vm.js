define('vm',
    [
        'vm.account',
        'vm.admin',
        'vm.machines',
        'vm.requests',
        'vm.shell'
    ],
    function (acount, admin, machines, requests, shell) {
        return {
            acount: acount,
            admin: admin,
            machines: machines,
            shell: shell,
            requests: requests,
        };
    });