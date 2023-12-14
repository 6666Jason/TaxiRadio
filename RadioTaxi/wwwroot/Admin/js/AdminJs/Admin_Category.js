var Admin_Cate = new Vue({
    el: "#Admin_Cate",
    data: {
        nameCategory: "",
        monthChoose: "",
        id: "",
        dataCategory:  [],
    },
    mounted() {
        this.loadCateItems();

    },
    methods: {
        loadCateItems() {
            $('#preloader').fadeIn();
            let currentPage = 0;
            if ($.fn.DataTable.isDataTable('#category_table')) {
                currentPage = $('#category_table').DataTable().page();
                $('#category_table').DataTable().destroy();
            }

            axios.get("/AdminRadio/AdminCategoryPackage/GetAll")
                .then((response) => {
                    this.dataCategory = response.data;
                    $('#preloader').fadeOut();

                    return Promise.resolve();
                })
                .then(() => {
                    const table = $("#category_table").DataTable({
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
        async addCategory() {
            try {
                if (this.nameCategory === '') {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Vui lòng nhập chọn danh mục',
                        confirmButtonText: 'OK'
                    })
                    return;
                }
                const formData = new FormData();

                formData.append('Name', this.nameCategory);
                formData.append('DateSet', this.monthChoose);

                await axios.post('/AdminRadio/AdminCategoryPackage/Add', formData,
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
            axios.get(`/AdminRadio/AdminCategoryPackage/GetByID/${id}`)
                .then((response) => {
                    this.id = response.data.id;
                    this.nameCategory = response.data.name;
                    this.monthChoose = response.data.dateSet;
                    return Promise.resolve();
                });
        },
        resetDataCategory() {
            this.id = "";
            this.nameCategory = "";
            this.monthChoose = "";
        },
        async editCategory() {
            try {

                if (this.nameCategory === '') {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Vui lòng nhập chọn danh mục',
                        confirmButtonText: 'OK'
                    })
                    return;
                }
                const formData = new FormData();
                formData.append('Name', this.nameCategory);
                formData.append('DateSet', this.monthChoose);
                formData.append('ID', this.id);
                await axios.post('/AdminRadio/AdminCategoryPackage/Update', formData,
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
        getItemsByIdDelete(id) {
            axios.get(`/AdminRadio/AdminCategoryPackage/GetByID/${id}`)
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
                                axios.post('/AdminRadio/AdminCategoryPackage/Delete', formData, {
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
    }
});