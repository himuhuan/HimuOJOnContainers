<template>
    <div class="center-content-container">
        <slot />
    </div>
</template>

<script lang="ts" setup>
import { defineProps } from "vue";
const props = defineProps({
    topOffset: {
        required: false,
        type: String,
        default: "5vh",
    },
    backgroundColor: {
        required: false,
        type: String,
        default: "transparent",
    },
    width: {
        required: false,
        type: String,
        default: "500px",
    },
});
</script>

<style scoped>
.center-content-container {
    margin: v-bind("props.topOffset") auto;
    background-color: v-bind("props.backgroundColor");
    width: v-bind("props.width");
}
</style>
