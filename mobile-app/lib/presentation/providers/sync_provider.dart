import 'package:flutter/material.dart';
import 'package:sqflite/sqflite.dart';
import '../../data/sync_service.dart';
import '../../core/database_helper.dart';

class SyncProvider extends ChangeNotifier {
  final SyncService _syncService = SyncService();
  bool _isSyncing = false;
  int _pendingCount = 0;
  int _totalCount = 0;

  bool get isSyncing => _isSyncing;
  int get pendingCount => _pendingCount;
  int get totalCount => _totalCount;

  Future<void> checkPending() async {
    final db = await DatabaseHelper.instance.database;
    final count = Sqflite.firstIntValue(await db.rawQuery(
      "SELECT COUNT(*) FROM registros_coleta WHERE statusSincronizacao = 'PENDENTE'"
    ));
    _pendingCount = count ?? 0;

    final countTotal = Sqflite.firstIntValue(await db.rawQuery(
      "SELECT COUNT(*) FROM registros_coleta"
    ));
    _totalCount = countTotal ?? 0;
    
    notifyListeners();
  }

  Future<void> syncData() async {
    if (_pendingCount == 0) return;

    _isSyncing = true;
    notifyListeners();

    try {
      await _syncService.syncPendingRecords();
    } catch (e) {
      print('Erro no sync: $e');
    } finally {
      _isSyncing = false;
      await checkPending();
    }
  }

  Future<void> clearAllData() async {
    final db = await DatabaseHelper.instance.database;
    await db.transaction((txn) async {
      await txn.delete('registros_coleta');
      await txn.delete('matriculas');
      await txn.delete('alunos_responsaveis');
      await txn.delete('responsaveis');
      await txn.delete('alunos');
      await txn.delete('familias');
    });
    await checkPending();
  }
}
