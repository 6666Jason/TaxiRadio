var company_vue = new Vue({
    el: "#company_vue",
    data:{
        dataCompany: [],
        dataPackage: [],
        contactPerson:"",
        companyName:"",
        idCompany:"",
        designation:"",
        address:"",
        mobile:"",
        telephone:"",
        faxNumber:"",
        email:"",
        memberShipType:"",
        packageMainID: "",
        id: "",
        ckName: "",
        editor: "",

    
    },
    mounted() {
        this.loadCateItems();
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
            if ($.fn.DataTable.isDataTable('#company_table')) {
                currentPage = $('#company_table').DataTable().page();
                $('#company_table').DataTable().destroy();
            }

            axios.get("/AdminRadio/AdminCompany/GetAllCompany")
                .then((response) => {
                    this.dataCompany = response.data;
                    $('#preloader').fadeOut();

                    return Promise.resolve();
                })
                .then(() => {
                    const table = $("#company_table").DataTable({
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

                formData.append('IDCompany', this.idCompany);
                formData.append('CompanyName', this.companyName);
                formData.append('Designation', this.designation);
                formData.append('Address', this.address);
                formData.append('Mobile', this.mobile);
                formData.append('Telephone', this.telephone);
                formData.append('FaxNumber', this.faxNumber);
                formData.append('Email', this.email);
                formData.append('MemberShipType', this.memberShipType);
                formData.append('ContactPerson', this.contactPerson);
                formData.append('PackageId', this.packageMainID);

                await axios.post('/AdminRadio/AdminCompany/Add', formData,
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
                        this.loadCateItems();

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
            axios.get(`/AdminRadio/AdminCompany/GetByID/${id}`)
                .then((response) => {
                    this.id = response.data.id;
                    this.idCompany = response.data.idCompany;
                    this.contactPerson = response.data.contactPerson;
                    this.designation = response.data.designation;
                    this.address = response.data.address;
                    this.mobile = response.data.mobile;
                    this.telephone = response.data.telephone;
                    this.faxNumber = response.data.faxNumber;
                    this.email = response.data.email;
                    this.memberShipType = response.data.memberShipType;
                    this.packageMainID = response.data.packageId;
                    this.companyName = response.data.companyName;
                    
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
            this.idCompany = "";
            this.designation = "";
            this.address = "";
            this.mobile = "";
            this.telephone = "";
            this.faxNumber = "";
            this.email = "";
            this.memberShipType = "";
            this.companyName = "";
           
        },
        async editItems() {
            try {


                const formData = new FormData();
                formData.append('IDCompany', this.idCompany);
                formData.append('CompanyName', this.companyName);
                formData.append('Designation', this.designation);
                formData.append('Address', this.address);
                formData.append('Mobile', this.mobile);
                formData.append('Telephone', this.telephone);
                formData.append('FaxNumber', this.faxNumber);
                formData.append('Email', this.email);
                formData.append('MemberShipType', this.memberShipType);
                formData.append('ContactPerson', this.contactPerson);
                formData.append('PackageId', this.packageMainID);
                formData.append('ID', this.id);
                await axios.post('/AdminRadio/AdminCompany/Update', formData,
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
                        this.loadCateItems();


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
        ChangeAcive(item) {
            fetch(`/Admin/UserManager/ChangeActive/${item.applicationUserMain.id}`)
                .then(res => {
                    window.location.reload()
                })
        },
        getItemsByIdDelete(id) {
            axios.get(`/AdminRadio/AdminCompany/GetByID/${id}`)
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
                                axios.post('/AdminRadio/AdminCompany/Delete', formData, {
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
            console.log(items);
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
                if (!items.status) {

                    items.status = true;
                    formData.append('ID', items.id);

                    axios.post('/AdminRadio/AdminCompany/HandleTransaction', formData, {
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

                }
            } catch {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Error',
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
                                text: 'Error ',
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
                    text: 'Error ',
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