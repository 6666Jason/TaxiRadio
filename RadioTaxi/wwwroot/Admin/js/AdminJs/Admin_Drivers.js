var drivers_vue = new Vue({
    el: "#drivers_vue",
    data:{
        dataDrivers: [],
        dataPackage: [],
        idDriver:"",
        contactPerson:"",
        address:"",
        mobile:"",
        telephone:"",
        experience:"",
        email:"",
        description:"",
        packageMainID: "",
        id: "",
        ckName: "",
        editor: "",
        City: ""

    
    },
    mounted() {
        this.loadCateItems();
        configureCKEditor('#editorMain', this, this.ckName);
        axios.get("/AdminRadio/AdminPage/GetAllPackage")
            .then((response) => {
                this.dataPackage = response.data;
                return Promise.resolve();
            });
    },
    watch: {
        ckName(newVal, oldVal) {
            if (!this.editor && newVal !== oldVal) {
                this.openEditor();
            }
        }
    },
    beforeDestroy() {
        if (this.editor) {
            this.editor.destroy();
        }
    },

    methods: {
        ChangeAcive(item) {
            fetch(`/Admin/UserManager/ChangeActive/${item.applicationUserMain.id}`)
                .then(res => {
                    window.location.reload()
                })
        },
        openEditor() {
           
            if (!this.editor) {
                configureCKEditor('#editor', this, this.ckName || {});
            }
        },
        formatDate(date) {
            const options = { year: 'numeric', month: '2-digit', day: '2-digit' };
            return date.toLocaleDateString('vi-VN', options);
        },
        formatCurrency(amount) {
            const formatter = new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD'
            });

            return formatter.format(amount);
        },
        loadCateItems() {
            $('#preloader').fadeIn();
            let currentPage = 0;
            if ($.fn.DataTable.isDataTable('#drivers_table')) {
                currentPage = $('#drivers_table').DataTable().page();
                $('#drivers_table').DataTable().destroy();
            }

            axios.get("/AdminRadio/AdminDrivers/GetAll")
                .then((response) => {
                    this.dataDrivers = response.data;
                    $('#preloader').fadeOut();

                    return Promise.resolve();
                })
                .then(() => {
                    const table = $("#drivers_table").DataTable({
                        ...this.$globalConfig.createDataTableConfig(),
                        'columnDefs': [{
                            'targets': [-1],
                            'orderable': false,
                        }],
                        searching: true,
                        iDisplayLength: 7,
                        "ordering": false,
                        lengthChange: false,
                        aaSorting: [[0, "desc"]],
                        aLengthMenu: [
                            [5, 10, 25, 50, 100, -1],

                            ["5 dòng", "10 dòng", "25 dòng", "50 dòng", "100 dòng", "Tất cả"],
                        ]

                    });
                    if (currentPage !== 0) {
                        table.page(currentPage).draw('page');
                    }
                });
        },
        async addItems() {
            try {

                const formData = new FormData();

                formData.append('IDDriver', this.idDriver);
                formData.append('ContactPerson', this.contactPerson);
                formData.append('Address', this.address);
                formData.append('Mobile', this.mobile);
                formData.append('Telephone', this.telephone);
                formData.append('Experience', this.experience);
                formData.append('Email', this.email);
                formData.append('Description', this.ckName);
                formData.append('City', this.City);
                formData.append('PackageId', this.packageMainID);

                await axios.post('/AdminRadio/AdminDrivers/Add', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Success',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();

                    }
                });
            } catch (error) {
                console.error(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Đã có Error xảy ra',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            }
        },
        getItemsById(id) {
            axios.get(`/AdminRadio/AdminDrivers/GetByID/${id}`)
                .then((response) => {
                    this.id = response.data.id;
                    this.idDriver = response.data.idDriver;
                    this.contactPerson = response.data.contactPerson;
                    this.address = response.data.address;
                    this.mobile = response.data.mobile;
                    this.telephone = response.data.telephone;
                    this.experience = response.data.experience;
                    this.email = response.data.email;
                    this.description = response.data.description;
                    this.ckName = response.data.description;
                    this.packageMainID = response.data.packageId;
                    this.City = response.data.city;
                    this.initializeEditor()
                    return Promise.resolve();
                });
        },
        destroyEditor() {
            if (this.editor) {
                this.editor.destroy();
                this.editor = null;
            }
        },
        initializeEditor() {
                this.destroyEditor(); 
                if (this.ckName == "") {
                    configureCKEditor('#editor', this, this.ckName);
                }
        },

        resetData() {
            this.id = "";
            this.packageMainID = 0;
            this.idDriver = "";
            this.contactPerson = "";
            this.experience = "";
            this.address = "";
            this.mobile = "";
            this.telephone = "";
            this.faxNumber = "";
            this.email = "";
            this.ckName = "";
            this.City = "";
            this.description = "";
            $('#Edit').on('hidden.bs.modal', () => {
                this.destroyEditor();
            });
        },
        async editItems() {
            try {


                const formData = new FormData();
                formData.append('IDDriver', this.idDriver);
                formData.append('ContactPerson', this.contactPerson);
                formData.append('Address', this.address);
                formData.append('Mobile', this.mobile);
                formData.append('Telephone', this.telephone);
                formData.append('Experience', this.experience);
                formData.append('Email', this.email);
                formData.append('Description', this.ckName);
                formData.append('PackageId', this.packageMainID);
                formData.append('City', this.City);

                formData.append('ID', this.id);
                await axios.post('/AdminRadio/AdminDrivers/Update', formData,
                    {
                        headers: {
                            'Content-Type': 'multipart/form-data'
                        }
                    });
                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'Success',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();

                    }
                });
            } catch (error) {
                console.error(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Đã có Error xảy ra',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();

                    }
                });
            }
        },
        getItemsByIdDelete(id) {
            axios.get(`/AdminRadio/AdminDrivers/GetByID/${id}`)
                .then((response) => {
                    this.id = response.data.id;
                    if (this.id != null) {
                        Swal.fire({
                            title: 'Delete product',
                            text: 'Are you sure you want to delete',
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Agree',
                            cancelButtonText: 'No!!!'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                const formData = new FormData();
                                formData.append('ID', this.id);
                                axios.post('/AdminRadio/AdminDrivers/Delete', formData, {
                                    headers: {
                                        'Content-Type': 'application/x-www-form-urlencoded'
                                    }
                                }).then(response => {
                                    Swal.fire({
                                        icon: 'success',
                                        title: 'Success',
                                        text: 'Success',
                                        confirmButtonText: 'OK',
                                    }).then((result) => {
                                        if (result.isConfirmed) {
                                            window.location.reload();


                                        }
                                    });

                                }).catch(error => {
                                    Swal.fire({
                                        icon: 'error',
                                        title: 'Error',
                                        text: 'An error occurred, please try again',
                                        confirmButtonText: 'OK'
                                    });
                                });
                            } else {
                                return;
                            }
                        });
                    }
                }).catch((error) => {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'An error occurred, please try again',
                        confirmButtonText: 'OK'
                    });
                })
        },
        async handleTransaction(items) {
            Swal.fire({
                title: 'Đang xử lý...',
                allowOutsideClick: false,
                onBeforeOpen: () => {
                    Swal.showLoading();
                },
                showConfirmButton: false
            });
            const formData = new FormData();

            try {
                    items.status = true;
                    formData.append('ID', items.id);
                axios.post('/AdminRadio/AdminDrivers/HandleTransaction', formData, {
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }

                    }).then(res => {
                        if (res.data) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Success',
                                text: 'Success',
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.reload();
                                }
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: 'Error ',
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.reload();
                                }
                            });
                        }
                    })

                
            } catch {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error ',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            }



        },
        async handlePayment(items) {
            Swal.fire({
                title: 'Đang xử lý...',
                allowOutsideClick: false,
                onBeforeOpen: () => {
                    Swal.showLoading();
                },
                showConfirmButton: false
            });
            const formData = new FormData();
            try {
                if (!items.payment) {

                    items.status = true;
                    formData.append('ID', items.id);

                    axios.post('/AdminRadio/AdminCompany/HandlePayment', formData, {
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }

                    }).then(res => {
                        if (res.data) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Success',
                                text: 'Success',
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.reload();
                                }
                            });
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: 'Error trong quá trình gửi mail',
                                confirmButtonText: 'OK'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.reload();
                                }
                            });
                        }
                    })

                }
            } catch {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error trong quá trình gửi mail',
                    confirmButtonText: 'OK'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.reload();
                    }
                });
            }



        },
    }
});