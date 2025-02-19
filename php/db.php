<?php
header("Access-Control-Allow-Origin: *"); 
header("Access-Control-Allow-Methods: GET, POST, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type, Authorization");

$host = "lelyim7e.beget.tech"; // или другой адрес сервера MySQL
$dbname = "lelyim7e_nixzord";
$user = "lelyim7e_nixzord";
$pass = "141722A!a";
try {
    $pdo = new PDO("mysql:host=$host;dbname=$dbname;charset=utf8mb4", $user, $pass);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (PDOException $e) {
    die(json_encode(["error" => "Ошибка подключения к базе данных: " . $e->getMessage()]));
}
?>
